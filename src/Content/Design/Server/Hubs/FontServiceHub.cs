using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Font>>> FontFetchForContentPortal(Request<ContentPortal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
            await FontService.FetchForContentPortal(request));
    }

    public async Task<Response<Font?>> FontFetch(Request<Font> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
            await FontService.Fetch(request));
    }

    public async Task<Response<Font?>> FontAdd(Request<Font> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
        {
            var response = await FontService.Add(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Styles, new FontAdded
                {
                    Id = response.Value.Id,
                    ContentPortalId = request.Value.ContentPortalId,
                });

                if (request.Value.ContentPortalId.HasValue)
                    await GatewayService.Publish(new PortalRunChanged { Id = request.Value.ContentPortalId.Value });
            }

            return response;
        });
    }

    public async Task<Response> FontSave(Request<Font> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
        {
            var response = await FontService.Save(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Styles, new FontSaved
                {
                    Id = request.Value.Id,
                    ContentPortalId = request.Value.ContentPortalId,
                });

                if (request.Value.ContentPortalId.HasValue)
                    await GatewayService.Publish(new PortalRunChanged { Id = request.Value.ContentPortalId.Value });
            }

            return response;
        });
    }

    public async Task<Response> FontRemove(Request<Font> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Styles, async session =>
        {
            var response = await FontService.Remove(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Styles, new FontRemoved
                {
                    Id = request.Value.Id,
                    ContentPortalId = request.Value.ContentPortalId,
                });

                if (request.Value.ContentPortalId.HasValue)
                    await GatewayService.Publish(new PortalRunChanged { Id = request.Value.ContentPortalId.Value });
            }

            return response;
        });
    }

    public async Task<Response<IList<IconFull>>> FontFetchIcons(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await FontService.FetchIcons(request));
    }
}