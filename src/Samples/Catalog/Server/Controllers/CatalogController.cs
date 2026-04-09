using Azure.Messaging.EventGrid;
using Crudspa.Samples.Catalog.Server.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Samples.Catalog.Server.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(
    IHubContext<CatalogHub> hubContext,
    ICacheService cacheService,
    ISessionFetcher sessionFetcher)
    : Crudspa.Framework.Core.Server.Controllers.GatewayController<CatalogHub>(hubContext, cacheService, sessionFetcher)
{
    protected override Boolean PublishPortalRun => true;

    [HttpPost("events")]
    public Task<IActionResult> HandleEvents([FromBody] EventGridEvent[] events) =>
        Handle(events);

    protected override Task<Boolean> TryHandle(EventGridEvent gridEvent) =>
        Crudspa.Framework.Jobs.Server.Services.JobGatewayRelay.TryHandle(gridEvent, HubContext);
}