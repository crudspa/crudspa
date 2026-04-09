namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IServerConfigService
{
    ServerConfig Fetch();
    void Invalidate();
}