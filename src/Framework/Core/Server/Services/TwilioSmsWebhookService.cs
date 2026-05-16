using System.Net.Http.Headers;
using Twilio.Security;

namespace Crudspa.Framework.Core.Server.Services;

public class TwilioSmsWebhookService(
    ILogger<TwilioSmsWebhookService> logger,
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ISmsChannelService smsChannelService,
    IBlobService blobService,
    IFileService fileService)
    : ISmsWebhookService
{
    private const Int32 ProviderTwilio = 0;
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
    private readonly HttpClient _httpClient = new();
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
            var providerMessageId = Read(webhook, "MessageSid", "SmsMessageSid", "SmsSid");
            var providerStatus = Read(webhook, "MessageStatus", "SmsStatus");
            var idempotencyKey = $"twilio:{channel.Key}:message:{providerMessageId ?? Guid.NewGuid().ToString()}";
            var signatureValid = ValidateSignature(webhook, channel);

            var eventStart = await SmsWebhookEventStart.Execute(
                Config.Database,
                SystemSessionId,
                channel.Key,
                ProviderTwilio,
                EventTypeInboundMessage,
                idempotencyKey,
                providerMessageId,
                providerStatus,
                webhook.RequestUrl,
                webhook.RequestSignature,
                signatureValid,
                webhook.Form.ToJson() ?? "{}",
                signatureValid ? EventStatusReceived : EventStatusFailed,
                signatureValid ? null : "Invalid Twilio request signature.");

            if (!signatureValid)
            {
                response.AddError("Invalid Twilio request signature.");
                return;
            }

            if (eventStart.Duplicate)
            {
                logger.LogInformation("Ignoring duplicate Twilio inbound SMS webhook. MessageSid: {messageSid}", providerMessageId);
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
                    ProviderTwilio,
                    providerMessageId,
                    fromNumber,
                    toNumber,
                    body,
                    providerStatus,
                    segmentCount);

                await ProcessMedia(webhook, channel, smsMessageId);
            }
            catch (Exception ex)
            {
                finalStatus = EventStatusFailed;
                errorMessage = ex.Message;
                logger.LogError(ex, "Exception while processing Twilio inbound SMS webhook. MessageSid: {messageSid}", providerMessageId);
                response.AddError("Twilio inbound SMS webhook processing failed.");
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
            var providerMessageId = Read(webhook, "MessageSid", "SmsMessageSid", "SmsSid");
            var providerStatus = Read(webhook, "MessageStatus", "SmsStatus");
            var idempotencyKey = $"twilio:{channel.Key}:status:{providerMessageId ?? Guid.NewGuid().ToString()}:{providerStatus ?? "unknown"}";
            var signatureValid = ValidateSignature(webhook, channel);

            var eventStart = await SmsWebhookEventStart.Execute(
                Config.Database,
                SystemSessionId,
                channel.Key,
                ProviderTwilio,
                EventTypeStatusCallback,
                idempotencyKey,
                providerMessageId,
                providerStatus,
                webhook.RequestUrl,
                webhook.RequestSignature,
                signatureValid,
                webhook.Form.ToJson() ?? "{}",
                signatureValid ? EventStatusReceived : EventStatusFailed,
                signatureValid ? null : "Invalid Twilio request signature.");

            if (!signatureValid)
            {
                response.AddError("Invalid Twilio request signature.");
                return;
            }

            if (eventStart.Duplicate)
            {
                logger.LogInformation("Ignoring duplicate Twilio status webhook. MessageSid: {messageSid}; Status: {status}", providerMessageId, providerStatus);
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
                    ProviderTwilio,
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
                logger.LogError(ex, "Exception while processing Twilio status webhook. MessageSid: {messageSid}; Status: {status}", providerMessageId, providerStatus);
                response.AddError("Twilio status webhook processing failed.");
            }
            finally
            {
                await SmsWebhookEventComplete.Execute(Config.Database, SystemSessionId, eventStart.Id, smsMessageId, finalStatus, errorMessage);
            }
        });
    }

    private Boolean ValidateSignature(TwilioSmsWebhookRequest request, SmsChannelConfig channel)
    {
        var authToken = TwilioAuthToken(channel);

        if (request.RequestUrl.HasNothing() || request.RequestSignature.HasNothing() || authToken.HasNothing())
            return false;

        var validator = new RequestValidator(authToken);
        return validator.Validate(GetValidationUrl(request, channel), request.Form, request.RequestSignature);
    }

    private String? GetValidationUrl(TwilioSmsWebhookRequest request, SmsChannelConfig channel)
    {
        var publicBaseUrl = channel.PublicBaseUrl;

        if (publicBaseUrl.HasNothing() || request.RequestUrl.HasNothing())
            return request.RequestUrl;

        if (!Uri.TryCreate(request.RequestUrl, UriKind.Absolute, out var incoming))
            return request.RequestUrl;

        var builder = new UriBuilder(publicBaseUrl)
        {
            Path = incoming.AbsolutePath,
            Query = incoming.Query.TrimStart('?'),
        };

        return builder.Uri.ToString();
    }

    private async Task ProcessMedia(TwilioSmsWebhookRequest request, SmsChannelConfig channel, Guid? smsMessageId)
    {
        if (!smsMessageId.HasValue)
            return;

        var mediaCount = ReadInt(request, "NumMedia") ?? 0;

        for (var i = 0; i < mediaCount; i++)
        {
            var mediaUrl = Read(request, $"MediaUrl{i}");
            var contentType = Read(request, $"MediaContentType{i}");
            var providerMediaId = ReadProviderMediaId(mediaUrl);
            var fileName = BuildFileName(providerMediaId, contentType, i);

            if (mediaUrl.HasNothing())
            {
                await SaveMedia(smsMessageId, null, providerMediaId, mediaUrl, contentType, fileName, null, MediaStatusFailed, "Missing Twilio media URL.", i);
                continue;
            }

            if (!IsAllowedInboundImage(contentType))
            {
                await SaveMedia(smsMessageId, null, providerMediaId, mediaUrl, contentType, fileName, null, MediaStatusSkipped, "Unsupported inbound media content type.", i);
                continue;
            }

            try
            {
                var download = await DownloadMedia(channel, mediaUrl!, contentType);
                fileName = BuildFileName(providerMediaId, download.ContentType, i);

                var blobId = Guid.NewGuid();
                await blobService.Add(new() { Id = blobId, Data = download.Data });

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

                await SaveMedia(smsMessageId, imageResponse.Value.Id, providerMediaId, mediaUrl, download.ContentType, fileName, download.Data.Length, MediaStatusDownloaded, null, i);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while downloading Twilio SMS media. Url: {mediaUrl}", mediaUrl);
                await SaveMedia(smsMessageId, null, providerMediaId, mediaUrl, contentType, fileName, null, MediaStatusFailed, ex.Message, i);
            }
        }
    }

    private async Task<(Byte[] Data, String ContentType)> DownloadMedia(SmsChannelConfig channel, String mediaUrl, String? fallbackContentType)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, mediaUrl);
        var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{TwilioCredentialSid(channel)}:{TwilioCredentialSecret(channel)}"));
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        using var httpResponse = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        httpResponse.EnsureSuccessStatusCode();

        var contentType = httpResponse.Content.Headers.ContentType?.MediaType ?? fallbackContentType ?? String.Empty;

        if (!IsAllowedInboundImage(contentType))
            throw new($"Unsupported inbound media content type: {contentType}");

        var contentLength = httpResponse.Content.Headers.ContentLength;
        if (contentLength is > MaxMediaBytes)
            throw new($"Inbound media is too large. Size: {contentLength}; Limit: {MaxMediaBytes}.");

        await using var stream = await httpResponse.Content.ReadAsStreamAsync();
        using var memory = new MemoryStream();
        var buffer = new Byte[81920];
        Int32 read;

        while ((read = await stream.ReadAsync(buffer)) > 0)
        {
            if (memory.Length + read > MaxMediaBytes)
                throw new($"Inbound media is too large. Limit: {MaxMediaBytes}.");

            memory.Write(buffer, 0, read);
        }

        return (memory.ToArray(), contentType);
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
            throw new($"Twilio webhook value is required: {key}");

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

    private static String? ReadProviderMediaId(String? mediaUrl)
    {
        if (mediaUrl.HasNothing() || !Uri.TryCreate(mediaUrl, UriKind.Absolute, out var uri))
            return null;

        return uri.Segments.LastOrDefault()?.TrimEnd('/');
    }

    private static String BuildFileName(String? providerMediaId, String? contentType, Int32 ordinal)
    {
        var extension = contentType.HasSomething() ? HttpResponseEx.GetExtension(contentType!, false) : String.Empty;

        if (extension.HasNothing())
            extension = ".bin";

        return $"{(providerMediaId.HasSomething() ? providerMediaId : $"twilio-media-{ordinal}")}{extension}";
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
            _ => MessageStatusSent,
        };
    }

    private String? TwilioCredentialSid(SmsChannelConfig channel) =>
        channel.TwilioApiKeySid.HasSomething()
            ? channel.TwilioApiKeySid
            : channel.TwilioAccountSid;

    private String? TwilioCredentialSecret(SmsChannelConfig channel) =>
        channel.TwilioApiKeySecret.HasSomething()
            ? channel.TwilioApiKeySecret
            : TwilioAuthToken(channel);

    private String? TwilioAuthToken(SmsChannelConfig channel) =>
        channel.TwilioAuthToken;
}