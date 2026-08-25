namespace Crudspa.Content.Messaging.Server.Services;

public class SmsMessageServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISmsChannelService smsChannelService,
    ISmsSender smsSender)
    : ISmsMessageService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SmsMessage>>> SearchForPortal(Request<SmsMessageSearch> request)
    {
        return await wrappers.Try<IList<SmsMessage>>(request, async response =>
        {
            var smsMessages = await SmsMessageSelectWhereForPortal.Execute(Connection, request.SessionId, request.Value);

            return smsMessages;
        });
    }

    public async Task<Response<IList<SmsMessage>>> SearchForMembership(Request<SmsMessageSearch> request)
    {
        return await wrappers.Try<IList<SmsMessage>>(request, async response =>
        {
            var smsMessages = await SmsMessageSelectWhereForMembership.Execute(Connection, request.SessionId, request.Value);

            return smsMessages;
        });
    }

    public async Task<Response<IList<SmsMessage>>> SearchForContactPhone(Request<SmsMessageSearch> request)
    {
        return await wrappers.Try<IList<SmsMessage>>(request, async response =>
        {
            var smsMessages = await SmsMessageSelectWhereForContactPhone.Execute(Connection, request.SessionId, request.Value);

            return smsMessages;
        });
    }

    public async Task<Response<IList<SmsMessage>>> SearchForContact(Request<SmsMessageSearch> request)
    {
        return await wrappers.Try<IList<SmsMessage>>(request, async response =>
        {
            var smsMessages = await SmsMessageSelectWhereForContact.Execute(Connection, request.SessionId, request.Value);

            return smsMessages;
        });
    }

    public async Task<Response<IList<SmsMessage>>> SearchThread(Request<SmsMessage> request)
    {
        return await wrappers.Try<IList<SmsMessage>>(request, async response =>
        {
            var smsMessages = await SmsMessageSelectThread.Execute(Connection, request.SessionId, request.Value.Id);

            return smsMessages;
        });
    }

    public async Task<Response<SmsMessage?>> Fetch(Request<SmsMessage> request)
    {
        return await wrappers.Try<SmsMessage?>(request, async response =>
        {
            var smsMessage = await SmsMessageSelect.Execute(Connection, request.SessionId, request.Value);

            return smsMessage;
        });
    }

    public async Task<Response<SmsMessage?>> Reply(Request<SmsMessage> request)
    {
        return await wrappers.Validate<SmsMessage?, SmsMessage>(request, async response =>
        {
            var reply = request.Value;
            var source = await SmsMessageSelect.Execute(Connection, request.SessionId, new() { Id = reply.Id });

            if (source is null)
            {
                response.AddError("The selected text message could not be found.");
                return null;
            }

            var channel = smsChannelService.Resolve(source.SmsChannelKey, source.PortalId);
            var toNumber = source.Direction == SmsMessage.Directions.Inbound ? source.FromNumber : source.ToNumber;

            if (toNumber.HasNothing())
            {
                response.AddError("The selected text thread does not have a reply phone number.");
                return null;
            }

            var outboundMessage = new SmsOutboundMessage
            {
                Id = Guid.NewGuid(),
                SmsChannelKey = channel.Key,
                PortalId = source.PortalId,
                From = channel.FromNumber,
                To = NormalizePhone(toNumber),
                Body = reply.Body,
            };

            var sendResponse = await smsSender.Send(new(outboundMessage));
            var smsMessage = new SmsMessage
            {
                Id = outboundMessage.Id,
                SmsChannelKey = channel.Key,
                PortalId = source.PortalId,
                MembershipId = source.MembershipId,
                MemberId = source.MemberId,
                ContactId = source.ContactId,
                ContactPhoneId = source.ContactPhoneId,
                Direction = SmsMessage.Directions.Outbound,
                Body = reply.Body,
                FromNumber = outboundMessage.From,
                ToNumber = outboundMessage.To,
                Occurred = DateTimeOffset.Now,
                Status = sendResponse.Ok ? SmsMessage.Statuses.Sent : SmsMessage.Statuses.Failed,
                Provider = ResolveProvider(channel),
                ProviderMessageId = outboundMessage.ProviderMessageId,
                ApiResponse = sendResponse.ErrorMessages,
            };

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                smsMessage.Id = await SmsMessageInsert.Execute(connection, transaction, request.SessionId, smsMessage);
            });

            if (!sendResponse.Ok)
                response.AddErrors(sendResponse.Errors);

            return smsMessage;
        });
    }

    private static SmsMessage.Providers ResolveProvider(SmsChannelConfig channel)
    {
        var provider = SmsChannelServiceCore.ResolveProvider(channel.Provider);

        if (provider.Contains("Twilio", StringComparison.OrdinalIgnoreCase))
            return SmsMessage.Providers.Twilio;

        if (provider.Contains("LocalFile", StringComparison.OrdinalIgnoreCase))
            return SmsMessage.Providers.LocalFile;

        return SmsMessage.Providers.Mock;
    }

    private static String? NormalizePhone(String? phone)
    {
        if (phone.HasNothing())
            return phone;

        var digits = new String(phone!.Where(Char.IsDigit).ToArray());

        return digits.Length == 10 ? $"+1{digits}" : $"+{digits}";
    }
}