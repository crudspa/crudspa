using PermissionIds = Crudspa.Samples.Catalog.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Catalog.Server.Hubs;

public partial class CatalogHub
{
    public async Task<Response<IList<Shirt>>> ShirtSearch(Request<ShirtSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await ShirtService.Search(request);
        });
    }

    public async Task<Response<Shirt?>> ShirtFetch(Request<Shirt> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
            await ShirtService.Fetch(request));
    }

    public async Task<Response<Shirt?>> ShirtAdd(Request<Shirt> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            var response = await ShirtService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Shirts, new ShirtAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ShirtSave(Request<Shirt> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            var response = await ShirtService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Shirts, new ShirtSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ShirtRemove(Request<Shirt> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            var response = await ShirtService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Shirts, new ShirtRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ShirtFetchBrandNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
            await ShirtService.FetchBrandNames(request));
    }
}