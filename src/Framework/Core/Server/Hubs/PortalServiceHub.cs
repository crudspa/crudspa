using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<IList<Portal>>> PortalFetchAll(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await PortalService.FetchAll(request));
    }

    public async Task<Response<Portal?>> PortalFetch(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
            await PortalService.Fetch(request));
    }

    public async Task<Response> PortalSave(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Portals, async session =>
        {
            var response = await PortalService.Save(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Portals, new PortalSaved
                {
                    Id = request.Value.Id,
                });

                await GatewayService.Publish(new PortalRunChanged { Id = request.Value.Id });
            }

            return response;
        });
    }
}