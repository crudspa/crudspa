using PermissionIds = Crudspa.Content.Design.Shared.Contracts.Ids.PermissionIds;


namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<PublicationGroup>>> PublicationGroupFetchForPortal(Request<Portal> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationGroupService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
            await service.FetchForPortal(request));
    }

    public async Task<Response<PublicationGroup?>> PublicationGroupFetch(Request<PublicationGroup> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationGroupService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
            await service.Fetch(request));
    }

    public async Task<Response<PublicationGroup?>> PublicationGroupAdd(Request<PublicationGroup> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationGroupService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
        {
            var response = await service.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publications, new PublicationGroupAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> PublicationGroupSave(Request<PublicationGroup> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationGroupService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
        {
            var response = await service.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publications, new PublicationGroupSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> PublicationGroupRemove(Request<PublicationGroup> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationGroupService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
        {
            var response = await service.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publications, new PublicationGroupRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> PublicationGroupSaveOrder(Request<IList<PublicationGroup>> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationGroupService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
        {
            var response = await service.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publications, new PublicationGroupsReordered
                {
                    PortalId = request.Value.First().PortalId,
                });

            return response;
        });
    }
}