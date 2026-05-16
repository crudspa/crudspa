namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISmsWebhookService
{
    Task<Response> ReceiveInboundMessage(Request<TwilioSmsWebhookRequest> request);
    Task<Response> ReceiveStatusCallback(Request<TwilioSmsWebhookRequest> request);
}