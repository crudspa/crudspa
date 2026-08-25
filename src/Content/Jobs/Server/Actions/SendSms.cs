using Crudspa.Content.Jobs.Shared.Contracts.Config;

namespace Crudspa.Content.Jobs.Server.Actions;

public class SendSms(
    ILogger<SendSms> logger,
    IServerConfigService serverConfig,
    ISmsChannelService smsChannelService,
    IBlobService blobService,
    ISmsSender smsSender,
    IContentActionService contentActionService)
    : IJobAction
{
    public SendSmsConfig? Config { get; set; }
    public ServerConfig? ServerConfig { get; set; }

    private Guid? _sessionId;

    public void Configure(Guid? sessionId, String json)
    {
        _sessionId = sessionId;

        Config = json.FromJson<SendSmsConfig>() ?? new();
        ServerConfig = serverConfig.Fetch();
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            if (Config is null)
                throw new("Config is null.");

            var errors = Config.Validate();

            if (errors.HasItems())
                throw new("Config is invalid. " + errors.ToStringWithSpaces());

            logger.LogInformation("Fetching scheduled SMS messages...");

            var smsResponse = await contentActionService.FetchSmsForSending(new(_sessionId));

            if (!smsResponse.Ok)
                throw new("Call to IContentActionService.FetchSmsForSending() failed. " + smsResponse.ErrorMessages);

            var smsMessages = smsResponse.Value;

            logger.LogInformation("Found {smsCount} SMS messages to send.", smsMessages.Count);

            foreach (var sms in smsMessages)
            {
                logger.LogInformation("Processing SMS {smsId} ({attachmentCount} attachments)...", sms.Id, sms.SmsAttachments.Count);
                var channel = smsChannelService.Resolve(sms.SmsChannelKey, sms.PortalId);

                var membersResponse = await contentActionService.FetchMembers(new(_sessionId, new() { Id = sms.MembershipId, SmsChannelKey = channel.Key }));

                if (!membersResponse.Ok)
                    throw new("Call to IContentActionService.FetchMembers() failed. " + membersResponse.ErrorMessages);

                var members = membersResponse.Value;

                try
                {
                    var succeeded = 0;
                    var failed = 0;

                    foreach (var member in members)
                    {
                        var contactPhone = member.Contact.Phones.FirstOrDefault(x => x.SupportsSms == true && x.Phone.HasSomething());

                        if (contactPhone is null)
                        {
                            logger.LogInformation("Skipping member {memberId}; no SMS-capable phone number.", member.Id);
                            continue;
                        }

                        var body = ReplaceTokens(sms.Body, member.TokenValues);
                        var smsMessageMedias = new List<SmsMessageMedia>();
                        var outboundMessage = new SmsOutboundMessage
                        {
                            Id = Guid.NewGuid(),
                            SmsChannelKey = channel.Key,
                            PortalId = sms.PortalId,
                            From = channel.FromNumber,
                            To = NormalizePhone(contactPhone.Phone),
                            Body = body,
                        };

                        foreach (var attachment in sms.SmsAttachments)
                        {
                            var blob = await blobService.Fetch(new() { Id = attachment.ImageFile.BlobId });

                            if (blob?.Data is not null)
                            {
                                var contentType = ContentType(attachment.ImageFile.Format);
                                var mediaUrl = BuildMediaUrl(channel, attachment.ImageFile);

                                outboundMessage.Media.Add(new()
                                {
                                    Name = attachment.ImageFile.Name,
                                    ContentType = contentType,
                                    Url = mediaUrl,
                                    Data = blob.Data,
                                });

                                smsMessageMedias.Add(new()
                                {
                                    ImageFile = attachment.ImageFile,
                                    ProviderMediaUrl = mediaUrl,
                                    ContentType = contentType,
                                    FileName = attachment.ImageFile.Name,
                                    SizeBytes = blob.Data.Length,
                                    DownloadStatus = SmsMessageMedia.DownloadStatuses.Downloaded,
                                    Ordinal = attachment.Ordinal ?? smsMessageMedias.Count,
                                });
                            }
                        }

                        logger.LogInformation("Sending SMS to {contactName} ({contactPhone})...", member.Contact.Name, outboundMessage.To);

                        var sendResponse = await smsSender.Send(new(outboundMessage));

                        var smsMessage = new SmsMessage
                        {
                            SmsId = sms.Id,
                            SmsChannelKey = channel.Key,
                            MembershipId = sms.MembershipId,
                            MemberId = member.Id,
                            ContactId = member.Contact.Id,
                            ContactPhoneId = contactPhone.Id,
                            Direction = SmsMessage.Directions.Outbound,
                            Body = body,
                            FromNumber = outboundMessage.From,
                            ToNumber = outboundMessage.To,
                            Occurred = DateTimeOffset.Now,
                            Provider = ResolveProvider(channel),
                            ProviderMessageId = outboundMessage.ProviderMessageId,
                            ApiResponse = sendResponse.ErrorMessages,
                            SmsMessageMedias = smsMessageMedias.ToObservable(),
                        };

                        if (sendResponse.Ok)
                        {
                            succeeded++;
                            smsMessage.Status = SmsMessage.Statuses.Sent;
                        }
                        else
                        {
                            failed++;
                            smsMessage.Status = SmsMessage.Statuses.Failed;
                        }

                        await contentActionService.SaveSmsMessage(new(_sessionId, smsMessage));
                    }

                    logger.LogInformation("SMS action complete. Succeeded: {succeeded} | Failed: {failed}", succeeded, failed);
                    sms.Status = failed > 0 ? Sms.Statuses.Failed : Sms.Statuses.Sent;
                }
                catch (Exception ex)
                {
                    sms.Status = Sms.Statuses.Failed;
                    logger.LogError(ex, "Unexpected error while sending SMS messages.");
                }
                finally
                {
                    await contentActionService.UpdateStatus(new Request<Sms>(_sessionId, sms));
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending SMS messages.");
            return false;
        }
    }

    private SmsMessage.Providers ResolveProvider(SmsChannelConfig channel)
    {
        var provider = SmsChannelServiceCore.ResolveProvider(channel.Provider);

        if (provider.Contains("Twilio", StringComparison.OrdinalIgnoreCase))
            return SmsMessage.Providers.Twilio;

        if (provider.Contains("LocalFile", StringComparison.OrdinalIgnoreCase))
            return SmsMessage.Providers.LocalFile;

        return SmsMessage.Providers.Mock;
    }

    private String? BuildMediaUrl(SmsChannelConfig channel, ImageFile imageFile)
    {
        var relativeUrl = imageFile.FetchUrl();
        if (relativeUrl.HasNothing())
            return null;

        var baseUrl = channel.PublicBaseUrl.HasSomething()
            ? channel.PublicBaseUrl
            : ServerConfig?.PortalUrl;

        if (baseUrl.HasNothing())
            return relativeUrl;

        return new Uri(new Uri(baseUrl!.TrimEnd('/') + "/"), relativeUrl!.TrimStart('/')).ToString();
    }

    private static String? NormalizePhone(String? phone)
    {
        if (phone.HasNothing())
            return phone;

        var digits = new String(phone!.Where(Char.IsDigit).ToArray());

        return digits.Length == 10 ? $"+1{digits}" : $"+{digits}";
    }

    private static String? ContentType(String? format)
    {
        return format?.ToLowerInvariant() switch
        {
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            "gif" => "image/gif",
            "webp" => "image/webp",
            _ => null,
        };
    }

    public String? ReplaceTokens(String? input, IEnumerable<TokenValue> tokenValues)
    {
        if (input.HasNothing())
            return null;

        var output = input;

        foreach (var tokenValue in tokenValues)
            output = output.Replace($"[{tokenValue.TokenKey}]", $"{tokenValue.Value}");

        return output;
    }
}