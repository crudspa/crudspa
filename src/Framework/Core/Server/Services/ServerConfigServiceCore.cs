namespace Crudspa.Framework.Core.Server.Services;

public class ServerConfigServiceCore(IConfiguration configuration) : IServerConfigService
{
    private ServerConfig? _serverConfig;

    public ServerConfig Fetch()
    {
        return _serverConfig ??= new()
        {
            AppInsightsConnection = configuration.ReadString("Crudspa.Framework.Core.Server.AppInsightsConnection", false),
            BlobService = configuration.ReadString("Crudspa.Framework.Core.Server.BlobService"),
            BuildNumber = configuration.ReadString("Crudspa.Framework.Core.Server.BuildNumber"),
            Database = configuration.ReadString("Crudspa.Framework.Core.Server.Database"),
            EmailFromAddress = configuration.ReadString("Crudspa.Framework.Core.Server.EmailFromAddress"),
            EmailFromName = configuration.ReadString("Crudspa.Framework.Core.Server.EmailFromName"),
            EmailSender = configuration.ReadString("Crudspa.Framework.Core.Server.EmailSender"),
            EventReceiverUrls = configuration.ReadString("Crudspa.Framework.Core.Server.EventReceiverUrls", false),
            EventTopicEndpoint = configuration.ReadString("Crudspa.Framework.Core.Server.EventTopicEndpoint", false),
            EventTopicKey = configuration.ReadString("Crudspa.Framework.Core.Server.EventTopicKey", false),
            PortalId = configuration.ReadGuid("Crudspa.Framework.Core.Server.PortalId"),
            PortalUrl = configuration.ReadString("Crudspa.Framework.Core.Server.PortalUrl"),
            SendGridApiKey = configuration.ReadString("Crudspa.Framework.Core.Server.SendGridApiKey", false),
            SignalRAppName = configuration.ReadString("Crudspa.Framework.Core.Server.SignalRAppName"),
            SignalRUseAzure = configuration.ReadBoolean("Crudspa.Framework.Core.Server.SignalRUseAzure"),
            StorageAccount = configuration.ReadString("Crudspa.Framework.Core.Server.StorageAccount"),
            StorageContainer = configuration.ReadString("Crudspa.Framework.Core.Server.StorageContainer"),
        };
    }

    public void Invalidate()
    {
        _serverConfig = null;
    }
}