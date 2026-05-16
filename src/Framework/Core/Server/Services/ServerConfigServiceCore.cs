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
            SmsChannels = ReadSmsChannels(),
            StorageAccount = configuration.ReadString("Crudspa.Framework.Core.Server.StorageAccount"),
            StorageContainer = configuration.ReadString("Crudspa.Framework.Core.Server.StorageContainer"),
        };
    }

    private List<SmsChannelConfig> ReadSmsChannels()
    {
        var json = configuration.ReadString("Crudspa.Framework.Core.Server.SmsChannelsJson", false);

        if (json.HasSomething())
            return json.FromJson<List<SmsChannelConfig>>() ?? [];

        return configuration
            .GetSection("Crudspa.Framework.Core.Server.SmsChannels")
            .GetChildren()
            .Select(ReadSmsChannel)
            .Where(x => x.Key.HasSomething())
            .ToList();
    }

    private static SmsChannelConfig ReadSmsChannel(IConfigurationSection section)
    {
        return new()
        {
            Key = section["Key"],
            Name = section["Name"],
            Provider = section["Provider"],
            Enabled = ReadBoolean(section, "Enabled", true),
            IsDefault = ReadBoolean(section, "IsDefault", false),
            PortalId = ReadGuid(section, "PortalId"),
            FromNumber = section["FromNumber"],
            PublicBaseUrl = section["PublicBaseUrl"],
            MessageWebhookUrl = section["MessageWebhookUrl"],
            StatusCallbackUrl = section["StatusCallbackUrl"],
            RedirectOutgoingSms = section["RedirectOutgoingSms"],
            MaxMessagesPerSecond = ReadInt(section, "MaxMessagesPerSecond"),
            TwilioAccountSid = section["TwilioAccountSid"],
            TwilioAuthToken = section["TwilioAuthToken"],
            TwilioApiKeySid = section["TwilioApiKeySid"],
            TwilioApiKeySecret = section["TwilioApiKeySecret"],
            TwilioMessagingServiceSid = section["TwilioMessagingServiceSid"],
        };
    }

    private static Boolean ReadBoolean(IConfigurationSection section, String key, Boolean fallback)
    {
        var value = section[key];
        return value.HasSomething() && Boolean.TryParse(value, out var parsed) ? parsed : fallback;
    }

    private static Guid? ReadGuid(IConfigurationSection section, String key)
    {
        var value = section[key];
        return value.HasSomething() && Guid.TryParse(value, out var parsed) ? parsed : null;
    }

    private static Int32? ReadInt(IConfigurationSection section, String key)
    {
        var value = section[key];
        return value.HasSomething() && Int32.TryParse(value, out var parsed) ? parsed : null;
    }

    public void Invalidate()
    {
        _serverConfig = null;
    }
}