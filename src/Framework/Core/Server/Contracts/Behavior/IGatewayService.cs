namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IGatewayService
{
    Task Publish<T>(T eventObject) where T : class;
}