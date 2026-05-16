namespace Crudspa.Framework.Core.Server.Services;

public class SmsChannelServiceCore(IServerConfigService configService) : ISmsChannelService
{
    private ServerConfig Config => configService.Fetch();

    public IReadOnlyList<SmsChannelConfig> Fetch()
    {
        return Config.SmsChannels
            .Where(x => x.Enabled)
            .ToList();
    }

    public SmsChannelConfig Resolve(String? key = null, Guid? portalId = null)
    {
        var channels = Fetch();

        if (key.HasSomething())
        {
            var channel = channels.FirstOrDefault(x => x.Key.IsBasically(key));
            if (channel is not null) return channel;

            throw new($"SMS channel '{key}' is not configured or is disabled.");
        }

        portalId ??= Config.PortalId == Guid.Empty ? null : Config.PortalId;

        if (portalId.HasValue)
        {
            var portalChannel = channels.FirstOrDefault(x => x.PortalId == portalId && x.IsDefault)
                ?? channels.FirstOrDefault(x => x.PortalId == portalId);

            if (portalChannel is not null)
                return portalChannel;
        }

        var defaultChannel = channels.FirstOrDefault(x => x.IsDefault)
            ?? channels.FirstOrDefault();

        return defaultChannel
            ?? throw new("No enabled SMS channels are configured.");
    }

    public static String ResolveProvider(String? provider)
    {
        if (provider.HasNothing())
            throw new("SMS channel provider is required.");

        if (provider!.Contains("Twilio", StringComparison.OrdinalIgnoreCase))
            return "Twilio";

        if (provider.Contains("LocalFile", StringComparison.OrdinalIgnoreCase))
            return "LocalFile";

        if (provider.Contains("Null", StringComparison.OrdinalIgnoreCase))
            return "Null";

        return provider;
    }
}