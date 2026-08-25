namespace Crudspa.Content.Jobs.Server.Services;

public class ContentActionServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ISqlWrappers sqlWrappers,
    IGatewayService gatewayService)
    : IContentActionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Email>>> FetchEmailsForSending(Request request)
    {
        return await wrappers.Try<IList<Email>>(request, async response =>
            await EmailSelectForSending.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Sms>>> FetchSmsForSending(Request request)
    {
        return await wrappers.Try<IList<Sms>>(request, async response =>
            await SmsSelectForSending.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Member>>> FetchMembers(Request<Membership> request)
    {
        return await wrappers.Try<IList<Member>>(request, async response =>
            await MemberSelectForSending.Execute(Connection, request.SessionId, request.Value.Id, request.Value.SmsChannelKey));
    }

    public async Task<Response> SaveLog(Request<EmailLog> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await EmailLogInsert.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response> SaveSmsMessage(Request<SmsMessage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var smsMessage = request.Value;
                smsMessage.Id = await SmsMessageInsert.Execute(connection, transaction, request.SessionId, smsMessage);

                foreach (var smsMessageMedia in smsMessage.SmsMessageMedias)
                {
                    smsMessageMedia.SmsMessageId = smsMessage.Id;
                    await SmsMessageMediaInsert.Execute(connection, transaction, request.SessionId, smsMessageMedia);
                }

                await gatewayService.Publish(new SmsMessageAdded
                {
                    Id = smsMessage.Id,
                    MembershipId = smsMessage.MembershipId,
                    ContactId = smsMessage.ContactId,
                    ContactPhoneId = smsMessage.ContactPhoneId,
                });
            });
        });
    }

    public async Task<Response> UpdateStatus(Request<Email> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var email = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await EmailUpdateStatus.Execute(connection, transaction, request.SessionId, email.Id!.Value, email.Status);
            });
        });
    }

    public async Task<Response> UpdateStatus(Request<Sms> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var sms = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SmsUpdateStatus.Execute(connection, transaction, request.SessionId, sms.Id!.Value, sms.Status);

                await gatewayService.Publish(new SmsSaved
                {
                    Id = sms.Id,
                    MembershipId = sms.MembershipId,
                });
            });
        });
    }
}