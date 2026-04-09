namespace Crudspa.Framework.Core.Server.Contracts.Data;

public class ServerConfig
{
    public String AppInsightsConnection { get; set; } = null!;
    public String BlobService { get; set; } = null!;
    public String BuildNumber { get; set; } = null!;
    public String Database { get; set; } = null!;
    public String EmailFromAddress { get; set; } = null!;
    public String EmailFromName { get; set; } = null!;
    public String EmailSender { get; set; } = null!;
    public String EventReceiverUrls { get; set; } = String.Empty;
    public String EventTopicEndpoint { get; set; } = null!;
    public String EventTopicKey { get; set; } = null!;
    public Guid PortalId { get; set; } = Guid.Empty;
    public String PortalUrl { get; set; } = null!;
    public String SendGridApiKey { get; set; } = null!;
    public String SignalRAppName { get; set; } = null!;
    public Boolean SignalRUseAzure { get; set; } = false;
    public String StorageAccount { get; set; } = null!;
    public String StorageContainer { get; set; } = null!;
}