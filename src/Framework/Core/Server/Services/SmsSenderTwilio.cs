using System.Net.Http.Headers;
using System.Text.Json;

namespace Crudspa.Framework.Core.Server.Services;

public class SmsSenderTwilio : ISmsSender
{
    private static readonly TimeSpan SendInterval = TimeSpan.FromMilliseconds(100);
    private readonly SemaphoreSlim _sendGate = new(1, 1);
    private DateTimeOffset _nextSend = DateTimeOffset.MinValue;

    private readonly IServiceWrappers _wrappers;
    private readonly ISmsChannelService _smsChannelService;
    private readonly HttpClient _httpClient;

    public SmsSenderTwilio(IServiceWrappers wrappers, ISmsChannelService smsChannelService)
    {
        _wrappers = wrappers;
        _smsChannelService = smsChannelService;
        _httpClient = new();
    }

    public async Task<Response> Send(Request<SmsOutboundMessage> request)
    {
        return await _wrappers.Try(request, async response =>
        {
            var message = request.Value;
            var channel = _smsChannelService.Resolve(message.SmsChannelKey, message.PortalId);

            if (TwilioAccountSid(channel).HasNothing())
            {
                response.AddError($"SMS channel '{channel.Key}' does not have a Twilio Account SID.");
                return;
            }

            if (TwilioCredentialSid(channel).HasNothing() || TwilioCredentialSecret(channel).HasNothing())
            {
                response.AddError($"SMS channel '{channel.Key}' does not have complete Twilio credentials.");
                return;
            }

            var form = new List<KeyValuePair<String, String>>
            {
                new("To", message.To!),
                new("Body", message.Body ?? String.Empty),
            };

            if (TwilioMessagingServiceSid(channel).HasSomething())
                form.Add(new("MessagingServiceSid", TwilioMessagingServiceSid(channel)!));
            else
            {
                var fromNumber = message.From.HasSomething() ? message.From : channel.FromNumber;

                if (fromNumber.HasNothing())
                {
                    response.AddError($"SMS channel '{channel.Key}' does not have a From number or Twilio Messaging Service SID.");
                    return;
                }

                form.Add(new("From", fromNumber!));
            }

            var callbackUrl = message.StatusCallbackUrl.HasSomething()
                ? message.StatusCallbackUrl
                : channel.StatusCallbackUrl.HasSomething()
                    ? channel.StatusCallbackUrl
                    : null;

            if (callbackUrl.HasSomething())
                form.Add(new("StatusCallback", callbackUrl!));

            foreach (var media in message.Media)
            {
                if (media.Url.HasNothing())
                {
                    response.AddError($"SMS channel '{channel.Key}' is using Twilio, but media '{media.Name}' has no public URL.");
                    return;
                }

                form.Add(new("MediaUrl", media.Url!));
            }

            await Throttle(channel);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.twilio.com/2010-04-01/Accounts/{TwilioAccountSid(channel)}/Messages.json");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", BuildAuthorization(channel));
            httpRequest.Content = new FormUrlEncodedContent(form);

            var apiResponse = await _httpClient.SendAsync(httpRequest);
            var body = await apiResponse.Content.ReadAsStringAsync();

            if (!apiResponse.IsSuccessStatusCode)
                response.AddError($"Call to Twilio API failed. StatusCode: {apiResponse.StatusCode}. Response: {body}");
            else
                message.ProviderMessageId = ReadTwilioMessageSid(body);
        });
    }

    private static String? ReadTwilioMessageSid(String json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty("sid", out var sid) ? sid.GetString() : null;
        }
        catch
        {
            return null;
        }
    }

    private async Task Throttle(SmsChannelConfig channel)
    {
        await _sendGate.WaitAsync();

        try
        {
            var now = DateTimeOffset.UtcNow;

            if (now < _nextSend)
                await Task.Delay(_nextSend - now);

            _nextSend = DateTimeOffset.UtcNow.Add(SendIntervalForChannel(channel));
        }
        finally
        {
            _sendGate.Release();
        }
    }

    private static TimeSpan SendIntervalForChannel(SmsChannelConfig channel) =>
        channel.MaxMessagesPerSecond is > 0
            ? TimeSpan.FromSeconds(1.0 / channel.MaxMessagesPerSecond.Value)
            : SendInterval;

    private String BuildAuthorization(SmsChannelConfig channel) =>
        Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{TwilioCredentialSid(channel)}:{TwilioCredentialSecret(channel)}"));

    private String? TwilioAccountSid(SmsChannelConfig channel) =>
        channel.TwilioAccountSid;

    private String? TwilioCredentialSid(SmsChannelConfig channel) =>
        channel.TwilioApiKeySid.HasSomething()
            ? channel.TwilioApiKeySid
            : TwilioAccountSid(channel);

    private String? TwilioCredentialSecret(SmsChannelConfig channel) =>
        channel.TwilioApiKeySecret.HasSomething()
            ? channel.TwilioApiKeySecret
            : channel.TwilioAuthToken;

    private String? TwilioMessagingServiceSid(SmsChannelConfig channel) =>
        channel.TwilioMessagingServiceSid;
}