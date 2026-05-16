namespace Crudspa.Framework.Core.Server.Contracts.Data;

public class SmsChannelConfig
{
    public String? Key { get; set; }
    public String? Name { get; set; }
    public String? Provider { get; set; }
    public Boolean Enabled { get; set; } = true;
    public Boolean IsDefault { get; set; }
    public Guid? PortalId { get; set; }
    public String? FromNumber { get; set; }
    public String? PublicBaseUrl { get; set; }
    public String? MessageWebhookUrl { get; set; }
    public String? StatusCallbackUrl { get; set; }
    public String? RedirectOutgoingSms { get; set; }
    public Int32? MaxMessagesPerSecond { get; set; }
    public String? TwilioAccountSid { get; set; }
    public String? TwilioAuthToken { get; set; }
    public String? TwilioApiKeySid { get; set; }
    public String? TwilioApiKeySecret { get; set; }
    public String? TwilioMessagingServiceSid { get; set; }
}