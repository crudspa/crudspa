namespace Crudspa.Framework.Core.Server.Contracts.Data;

public class TwilioSmsWebhookRequest
{
    public String? SmsChannelKey { get; set; }
    public String? RequestUrl { get; set; }
    public String? RequestSignature { get; set; }
    public Dictionary<String, String> Form { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}