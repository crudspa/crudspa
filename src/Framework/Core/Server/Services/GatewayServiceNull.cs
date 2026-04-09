namespace Crudspa.Framework.Core.Server.Services;

public class GatewayServiceNull : IGatewayService
{
    public Task Publish<T>(T eventObject) where T : class => Task.CompletedTask;
}