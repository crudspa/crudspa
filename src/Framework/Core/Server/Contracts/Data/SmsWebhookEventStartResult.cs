namespace Crudspa.Framework.Core.Server.Contracts.Data;

public class SmsWebhookEventStartResult
{
    public Guid? Id { get; set; }
    public Boolean Duplicate { get; set; }
}