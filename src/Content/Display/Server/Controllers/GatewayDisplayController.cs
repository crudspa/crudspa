using Azure.Messaging.EventGrid;
using Crudspa.Content.Display.Server.Contracts;
using Crudspa.Framework.Core.Server.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Content.Display.Server.Controllers;

public abstract class GatewayDisplayController<THub>(
    IHubContext<THub> hubContext,
    ICacheService cacheService,
    ISessionFetcher sessionFetcher,
    IThemeRunService themeRunService)
    : Crudspa.Framework.Core.Server.Controllers.GatewayController<THub>(hubContext, cacheService, sessionFetcher)
    where THub : Hub
{
    private IThemeRunService ThemeRunService { get; } = themeRunService;

    protected override async Task HandlePortalRun(PortalRunChanged portalPayload)
    {
        await base.HandlePortalRun(portalPayload);

        if (!portalPayload.Id.HasValue)
            return;

        CacheService.Invalidate(StylesKeys.Revision(portalPayload.Id.Value));
        await ThemeRunService.Warm(portalPayload.Id.Value);
    }

    protected override async Task<Boolean> TryHandle(EventGridEvent gridEvent)
    {
        switch (gridEvent.Subject)
        {
            case "Crudspa.Content.Display.Shared.Contracts.Events.PageContentChanged":

                var pagePayload = gridEvent.Data.ToString().FromJson<PageContentChanged>();

                if (pagePayload is not null)
                {
                    CacheService.Invalidate(String.Format(CacheKeys.Page, pagePayload.Id));
                    await Publish(pagePayload);
                }

                return true;

            default:
                return await base.TryHandle(gridEvent);
        }
    }
}