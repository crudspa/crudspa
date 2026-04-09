using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Crudspa.Framework.Core.Server.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Framework.Core.Server.Controllers;

public abstract class GatewayController<THub>(
    IHubContext<THub> hubContext,
    ICacheService cacheService,
    ISessionFetcher sessionFetcher) : ControllerBase
    where THub : Hub
{
    protected IHubContext<THub> HubContext { get; } = hubContext;
    protected ICacheService CacheService { get; } = cacheService;
    protected ISessionFetcher SessionFetcher { get; } = sessionFetcher;
    protected virtual Boolean PublishPortalRun => false;

    protected async Task<IActionResult> Handle(EventGridEvent[] events)
    {
        foreach (var gridEvent in events)
        {
            if (TryValidateSubscriptionEvent(gridEvent, out var validationResponse))
                return validationResponse!;

            if (await TryHandleFramework(gridEvent))
                continue;

            if (await TryHandle(gridEvent))
                continue;
        }

        return Ok();
    }

    protected virtual Task<Boolean> TryHandle(EventGridEvent gridEvent) =>
        Task.FromResult(false);

    protected async Task Publish<T>(T eventObject) where T : class =>
        await HubContext.Clients.All.SendAsync("NoticePosted", new Notice<T>(eventObject));

    protected virtual async Task HandlePortalRun(PortalRunChanged portalPayload)
    {
        if (portalPayload.Id.HasValue != true)
            return;

        CacheService.Invalidate(String.Format(CacheKeys.Portal, portalPayload.Id.Value));

        if (PublishPortalRun)
            await Publish(portalPayload);
    }

    private static Boolean TryValidateSubscriptionEvent(EventGridEvent gridEvent, out IActionResult? actionResult)
    {
        actionResult = null;

        if (!gridEvent.TryGetSystemEventData(out var eventData))
            return false;

        if (eventData is not SubscriptionValidationEventData validationData)
            return false;

        actionResult = new OkObjectResult(new { validationResponse = validationData.ValidationCode });
        return true;
    }

    private async Task<Boolean> TryHandleFramework(EventGridEvent gridEvent)
    {
        switch (gridEvent.Subject)
        {
            case "Crudspa.Framework.Core.Shared.Contracts.Events.PortalRunChanged":

                var portalPayload = gridEvent.Data.ToString().FromJson<PortalRunChanged>();

                if (portalPayload is not null)
                    await HandlePortalRun(portalPayload);

                return true;

            case "Crudspa.Framework.Core.Shared.Contracts.Events.SessionsInvalidated":

                SessionFetcher.InvalidateAll();
                return true;

            default:
                return false;
        }
    }
}