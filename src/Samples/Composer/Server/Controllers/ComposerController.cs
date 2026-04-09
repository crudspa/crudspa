using Azure.Messaging.EventGrid;
using Crudspa.Samples.Composer.Server.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Samples.Composer.Server.Controllers;

[ApiController]
[Route("api/composer")]
public class ComposerController(
    IHubContext<ComposerHub> hubContext,
    ICacheService cacheService,
    ISessionFetcher sessionFetcher,
    IThemeRunService themeRunService)
    : Crudspa.Content.Display.Server.Controllers.GatewayDisplayController<ComposerHub>(hubContext, cacheService, sessionFetcher, themeRunService)
{
    [HttpPost("events")]
    public Task<IActionResult> HandleEvents([FromBody] EventGridEvent[] events) =>
        Handle(events);

    protected override async Task<Boolean> TryHandle(EventGridEvent gridEvent)
    {
        if (await Crudspa.Framework.Jobs.Server.Services.JobGatewayRelay.TryHandle(gridEvent, HubContext))
            return true;

        return await base.TryHandle(gridEvent);
    }
}