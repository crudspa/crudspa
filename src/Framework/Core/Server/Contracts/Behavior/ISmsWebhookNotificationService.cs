namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISmsWebhookNotificationService
{
    Task SmsMessageAdded(Guid? id);
    Task SmsMessageSaved(Guid? id);
    Task SmsEventSaved(Guid? id, Guid? smsMessageId);
}