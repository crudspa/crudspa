using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Catalog.Server.Hubs;

using Catalog = Shared.Contracts.Data.Catalog;

public partial class CatalogHub
{
    public async Task<Response<Catalog?>> CatalogFetch(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await CatalogService.Fetch(request));
    }

    public async Task<Response> CatalogSave(Request<Catalog> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await CatalogService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new CatalogSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> CatalogFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await CatalogService.FetchPermissionNames(request));
    }
}