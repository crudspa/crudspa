namespace Crudspa.Content.Messaging.Server.Services;

public class SmsWebhookNotificationServiceGateway(IGatewayService gatewayService) : ISmsWebhookNotificationService
{
    public async Task SmsMessageAdded(Guid? id)
    {
        if (!id.HasValue)
            return;

        await gatewayService.Publish(new SmsMessageAdded { Id = id });
    }

    public async Task SmsMessageSaved(Guid? id)
    {
        if (!id.HasValue)
            return;

        await gatewayService.Publish(new SmsMessageSaved { Id = id });
    }

    public async Task SmsEventSaved(Guid? id, Guid? smsMessageId)
    {
        if (!id.HasValue)
            return;

        await gatewayService.Publish(new SmsEventSaved
        {
            Id = id,
            SmsMessageId = smsMessageId,
        });
    }
}