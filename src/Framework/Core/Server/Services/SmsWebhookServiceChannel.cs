namespace Crudspa.Framework.Core.Server.Services;

public class SmsWebhookServiceChannel(
    IServiceWrappers wrappers,
    ISmsChannelService smsChannelService,
    SmsWebhookServiceNull nullService,
    SmsWebhookServiceLocalFile localFileService,
    TwilioSmsWebhookService twilioService)
    : ISmsWebhookService
{
    public Task<Response> ReceiveInboundMessage(Request<TwilioSmsWebhookRequest> request)
    {
        return Route(request, service => service.ReceiveInboundMessage(request));
    }

    public Task<Response> ReceiveStatusCallback(Request<TwilioSmsWebhookRequest> request)
    {
        return Route(request, service => service.ReceiveStatusCallback(request));
    }

    private async Task<Response> Route(Request<TwilioSmsWebhookRequest> request, Func<ISmsWebhookService, Task<Response>> handle)
    {
        return await wrappers.Try(request, async response =>
        {
            var webhook = request.Value;
            var channel = smsChannelService.Resolve(webhook.SmsChannelKey);

            if (!channel.Enabled)
            {
                response.AddError($"SMS channel '{channel.Key}' is disabled.");
                return;
            }

            webhook.SmsChannelKey = channel.Key;

            var webhookResponse = await handle(ResolveService(channel));

            response.AddErrors(webhookResponse.Errors);
        });
    }

    private ISmsWebhookService ResolveService(SmsChannelConfig channel)
    {
        var provider = SmsChannelServiceCore.ResolveProvider(channel.Provider);

        return provider.ToLowerInvariant() switch
        {
            "twilio" => twilioService,
            "localfile" => localFileService,
            "null" => nullService,
            _ => throw new($"SMS channel '{channel.Key}' uses an unknown provider '{channel.Provider}'."),
        };
    }
}