using PermissionIds = Crudspa.Content.Design.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

using Publication = Shared.Contracts.Data.Publication;

public partial class DesignHub
{
    public async Task<Response<IList<Publication>>> PublicationFetchForPortal(Request<Portal> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
            await service.FetchForPortal(request));
    }

    public async Task<Response<Publication?>> PublicationFetch(Request<Publication> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
            await service.Fetch(request));
    }

    public async Task<Response<Publication?>> PublicationAdd(Request<Publication> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
        {
            var response = await service.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publications, new PublicationAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> PublicationSave(Request<Publication> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
        {
            var response = await service.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publications, new PublicationSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> PublicationRemove(Request<Publication> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
        {
            var response = await service.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publications, new PublicationRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> PublicationSaveOrder(Request<IList<Publication>> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
        {
            var response = await service.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publications, new PublicationsReordered
                {
                    PortalId = request.Value.First().PortalId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> PublicationFetchPublicationGroupNames(Request<Portal> request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
            await service.FetchPublicationGroupNames(request));
    }

    public async Task<Response<IList<Orderable>>> PublicationFetchContentStatusNames(Request request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
            await service.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Orderable>>> PublicationFetchPublicationResourceTypeNames(Request request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationService service)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publications, async session =>
            await service.FetchPublicationResourceTypeNames(request));
    }
}