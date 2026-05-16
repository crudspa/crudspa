namespace Crudspa.Framework.Core.Server.Services;

public class SmsWebhookServiceNull(IServiceWrappers wrappers) : ISmsWebhookService
{
    public async Task<Response> ReceiveInboundMessage(Request<TwilioSmsWebhookRequest> request)
    {
        return await wrappers.Try(request, _ => Task.CompletedTask);
    }

    public async Task<Response> ReceiveStatusCallback(Request<TwilioSmsWebhookRequest> request)
    {
        return await wrappers.Try(request, _ => Task.CompletedTask);
    }
}