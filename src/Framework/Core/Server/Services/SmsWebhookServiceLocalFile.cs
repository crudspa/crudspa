namespace Crudspa.Framework.Core.Server.Services;

public class SmsWebhookServiceLocalFile(
    ILogger<SmsWebhookServiceLocalFile> logger,
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ISmsChannelService smsChannelService,
    IBlobService blobService,
    IFileService fileService)
    : ISmsWebhookService
{
    private const Int32 ProviderLocalFile = 1;
    private const Int32 MessageStatusQueued = 0;
    private const Int32 MessageStatusSending = 1;
    private const Int32 MessageStatusSent = 2;
    private const Int32 MessageStatusDelivered = 3;
    private const Int32 MessageStatusReceived = 4;
    private const Int32 MessageStatusUndelivered = 5;
    private const Int32 MessageStatusFailed = 6;
    private const Int32 EventTypeInboundMessage = 0;
    private const Int32 EventTypeStatusCallback = 1;
    private const Int32 EventStatusReceived = 0;
    private const Int32 EventStatusProcessed = 1;
    private const Int32 EventStatusFailed = 3;
    private const Int32 EventStatusIgnored = 4;
    private const Int32 MediaStatusDownloaded = 1;
    private const Int32 MediaStatusFailed = 2;
    private const Int32 MediaStatusSkipped = 3;
    private const Int32 MaxMediaBytes = 10 * 1024 * 1024;
    private static readonly Guid SystemSessionId = new("22f1a393-c003-4587-8f1d-02369d9c6c53");
    private static readonly HashSet<String> AllowedInboundImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
    };

    private ServerConfig Config => configService.Fetch();

    public async Task<Response> ReceiveInboundMessage(Request<TwilioSmsWebhookRequest> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var webhook = request.Value;
            var channel = smsChannelService.Resolve(webhook.SmsChannelKey);
            var providerMessageId = Read(webhook, "MessageSid", "SmsMessageSid", "SmsSid", "ProviderMessageId") ?? $"local-{Guid.NewGuid():D}";
            var providerStatus = Read(webhook, "MessageStatus", "SmsStatus") ?? "received";
            var eventStart = await SmsWebhookEventStart.Execute(
                Config.Database,
                SystemSessionId,
                channel.Key,
                ProviderLocalFile,
                EventTypeInboundMessage,
                $"localfile:{channel.Key}:message:{providerMessageId}",
                providerMessageId,
                providerStatus,
                webhook.RequestUrl,
                webhook.RequestSignature,
                true,
                webhook.Form.ToJson() ?? "{}",
                EventStatusReceived,
                null);

            if (eventStart.Duplicate)
            {
                logger.LogInformation("Ignoring duplicate local inbound SMS webhook. MessageSid: {messageSid}", providerMessageId);
                return;
            }

            Guid? smsMessageId = null;
            var finalStatus = EventStatusProcessed;
            String? errorMessage = null;

            try
            {
                var fromNumber = Require(webhook, "From");
                var toNumber = Require(webhook, "To");
                var body = Read(webhook, "Body");
                var segmentCount = ReadInt(webhook, "NumSegments");

                smsMessageId = await SmsWebhookMessageUpsert.Execute(
                    Config.Database,
                    SystemSessionId,
                    channel.Key,
                    channel.PortalId,
                    ProviderLocalFile,
                    providerMessageId,
                    fromNumber,
                    toNumber,
                    body,
                    providerStatus,
                    segmentCount);

                await ProcessMedia(webhook, smsMessageId);
            }
            catch (Exception ex)
            {
                finalStatus = EventStatusFailed;
                errorMessage = ex.Message;
                logger.LogError(ex, "Exception while processing local inbound SMS webhook. MessageSid: {messageSid}", providerMessageId);
                response.AddError("Local inbound SMS webhook processing failed.");
            }
            finally
            {
                await SmsWebhookEventComplete.Execute(Config.Database, SystemSessionId, eventStart.Id, smsMessageId, finalStatus, errorMessage);
            }
        });
    }

    public async Task<Response> ReceiveStatusCallback(Request<TwilioSmsWebhookRequest> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var webhook = request.Value;
            var channel = smsChannelService.Resolve(webhook.SmsChannelKey);
            var providerMessageId = Read(webhook, "MessageSid", "SmsMessageSid", "SmsSid", "ProviderMessageId");
            var providerStatus = Read(webhook, "MessageStatus", "SmsStatus") ?? "sent";
            var eventStart = await SmsWebhookEventStart.Execute(
                Config.Database,
                SystemSessionId,
                channel.Key,
                ProviderLocalFile,
                EventTypeStatusCallback,
                $"localfile:{channel.Key}:status:{providerMessageId ?? Guid.NewGuid().ToString()}:{providerStatus}",
                providerMessageId,
                providerStatus,
                webhook.RequestUrl,
                webhook.RequestSignature,
                true,
                webhook.Form.ToJson() ?? "{}",
                EventStatusReceived,
                null);

            if (eventStart.Duplicate)
            {
                logger.LogInformation("Ignoring duplicate local SMS status webhook. MessageSid: {messageSid}; Status: {status}", providerMessageId, providerStatus);
                return;
            }

            Guid? smsMessageId = null;
            var finalStatus = EventStatusProcessed;
            String? errorMessage = null;

            try
            {
                smsMessageId = await SmsWebhookStatusUpdate.Execute(
                    Config.Database,
                    SystemSessionId,
                    channel.Key,
                    ProviderLocalFile,
                    providerMessageId,
                    providerStatus,
                    MapProviderStatus(providerStatus),
                    Read(webhook, "ErrorCode"),
                    Read(webhook, "ErrorMessage"));

                if (!smsMessageId.HasValue)
                    finalStatus = EventStatusIgnored;
            }
            catch (Exception ex)
            {
                finalStatus = EventStatusFailed;
                errorMessage = ex.Message;
                logger.LogError(ex, "Exception while processing local SMS status webhook. MessageSid: {messageSid}; Status: {status}", providerMessageId, providerStatus);
                response.AddError("Local SMS status webhook processing failed.");
            }
            finally
            {
                await SmsWebhookEventComplete.Execute(Config.Database, SystemSessionId, eventStart.Id, smsMessageId, finalStatus, errorMessage);
            }
        });
    }

    private async Task ProcessMedia(TwilioSmsWebhookRequest request, Guid? smsMessageId)
    {
        if (!smsMessageId.HasValue)
            return;

        var mediaCount = ReadInt(request, "NumMedia") ?? 0;

        for (var i = 0; i < mediaCount; i++)
        {
            var localPath = ToLocalPath(Read(request, $"MediaPath{i}", $"MediaFile{i}", $"MediaUrl{i}"));
            var contentType = Read(request, $"MediaContentType{i}") ?? ContentTypeFromPath(localPath);
            var providerMediaId = localPath.HasSomething() ? Path.GetFileNameWithoutExtension(localPath) : null;
            var fileName = localPath.HasSomething() ? Path.GetFileName(localPath) : $"local-media-{i}.bin";

            if (localPath.HasNothing() || !File.Exists(localPath))
            {
                await SaveMedia(smsMessageId, null, providerMediaId, localPath, contentType, fileName, null, MediaStatusFailed, "Missing local media file.", i);
                continue;
            }

            if (!IsAllowedInboundImage(contentType))
            {
                await SaveMedia(smsMessageId, null, providerMediaId, localPath, contentType, fileName, null, MediaStatusSkipped, "Unsupported inbound media content type.", i);
                continue;
            }

            try
            {
                var fileInfo = new FileInfo(localPath);
                if (fileInfo.Length > MaxMediaBytes)
                    throw new($"Inbound media is too large. Size: {fileInfo.Length}; Limit: {MaxMediaBytes}.");

                var data = await File.ReadAllBytesAsync(localPath);
                var blobId = Guid.NewGuid();
                await blobService.Add(new() { Id = blobId, Data = data });

                var imageFile = new ImageFile
                {
                    BlobId = blobId,
                    Name = fileName,
                    Format = fileName.GetExtension(),
                    OptimizedStatus = ImageFile.OptimizationStatus.None,
                };

                var imageResponse = await fileService.SaveImage(new(SystemSessionId, imageFile));

                if (!imageResponse.Ok || imageResponse.Value?.Id is null)
                    throw new($"Image save failed. {imageResponse.ErrorMessages}");

                await SaveMedia(smsMessageId, imageResponse.Value.Id, providerMediaId, localPath, contentType, fileName, data.Length, MediaStatusDownloaded, null, i);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while importing local SMS media. Path: {localPath}", localPath);
                await SaveMedia(smsMessageId, null, providerMediaId, localPath, contentType, fileName, null, MediaStatusFailed, ex.Message, i);
            }
        }
    }

    private async Task SaveMedia(
        Guid? smsMessageId,
        Guid? imageId,
        String? providerMediaId,
        String? mediaUrl,
        String? contentType,
        String? fileName,
        Int32? sizeBytes,
        Int32 downloadStatus,
        String? errorMessage,
        Int32 ordinal)
    {
        await SmsWebhookMediaUpsert.Execute(
            Config.Database,
            SystemSessionId,
            smsMessageId,
            imageId,
            providerMediaId,
            mediaUrl,
            contentType,
            fileName,
            sizeBytes,
            downloadStatus,
            errorMessage,
            ordinal);
    }

    private static String Require(TwilioSmsWebhookRequest request, String key)
    {
        var value = Read(request, key);
        if (value.HasNothing())
            throw new($"Local webhook value is required: {key}");

        return value!;
    }

    private static String? Read(TwilioSmsWebhookRequest request, params String[] keys)
    {
        foreach (var key in keys)
            if (request.Form.TryGetValue(key, out var value) && value.HasSomething())
                return value;

        return null;
    }

    private static Int32? ReadInt(TwilioSmsWebhookRequest request, String key)
    {
        var value = Read(request, key);
        return Int32.TryParse(value, out var parsed) ? parsed : null;
    }

    private static Boolean IsAllowedInboundImage(String? contentType) =>
        contentType.HasSomething() && AllowedInboundImageTypes.Contains(contentType!);

    private static String? ToLocalPath(String? value)
    {
        if (value.HasNothing())
            return null;

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.IsFile)
            return uri.LocalPath;

        return value;
    }

    private static String? ContentTypeFromPath(String? path)
    {
        return (path?.GetExtension() ?? String.Empty).ToLowerInvariant() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => null,
        };
    }

    private static Int32 MapProviderStatus(String? providerStatus)
    {
        return providerStatus?.ToLowerInvariant() switch
        {
            "accepted" => MessageStatusQueued,
            "queued" => MessageStatusQueued,
            "scheduled" => MessageStatusQueued,
            "sending" => MessageStatusSending,
            "sent" => MessageStatusSent,
            "delivered" => MessageStatusDelivered,
            "undelivered" => MessageStatusUndelivered,
            "failed" => MessageStatusFailed,
            "canceled" => MessageStatusFailed,
            "received" => MessageStatusReceived,
            _ => MessageStatusSent,
        };
    }
}