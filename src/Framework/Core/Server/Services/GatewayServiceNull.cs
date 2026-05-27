namespace Crudspa.Framework.Core.Server.Services;

public class GatewayServiceNull : IGatewayService
{
    public Task Publish<T>(T eventObject, String eventRoute = GatewayEventRoutes.Broadcast) where T : class => Task.CompletedTask;
}