using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Forum>>> ForumFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await ForumService.FetchForPortal(request));
    }

    public async Task<Response<Forum?>> ForumFetch(Request<Forum> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await ForumService.Fetch(request));
    }

    public async Task<Response<Forum?>> ForumAdd(Request<Forum> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await ForumService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ForumAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> ForumSave(Request<Forum> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await ForumService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ForumSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> ForumRemove(Request<Forum> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await ForumService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ForumRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> ForumSaveOrder(Request<IList<Forum>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await ForumService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ForumsReordered
                {
                    PortalId = request.Value.First().PortalId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ForumFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await ForumService.FetchContentStatusNames(request));
    }
}