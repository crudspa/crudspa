namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISmsChannelService
{
    IReadOnlyList<SmsChannelConfig> Fetch();
    SmsChannelConfig Resolve(String? key = null, Guid? portalId = null);
}