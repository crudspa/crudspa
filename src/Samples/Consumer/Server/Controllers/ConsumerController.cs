using Azure.Messaging.EventGrid;
using Crudspa.Samples.Consumer.Server.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Samples.Consumer.Server.Controllers;

[ApiController]
[Route("api/consumer")]
public class ConsumerController(
    IHubContext<ConsumerHub> hubContext,
    ICacheService cacheService,
    ISessionFetcher sessionFetcher,
    IThemeRunService themeRunService)
    : Crudspa.Content.Display.Server.Controllers.GatewayDisplayController<ConsumerHub>(hubContext, cacheService, sessionFetcher, themeRunService)
{
    [HttpPost("events")]
    public Task<IActionResult> HandleEvents([FromBody] EventGridEvent[] events) =>
        Handle(events);
}