using PermissionIds = Crudspa.Samples.Catalog.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Samples.Catalog.Server.Hubs;

public partial class CatalogHub
{
    public async Task<Response<IList<ShirtOption>>> ShirtOptionFetchForShirt(Request<Shirt> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
            await ShirtOptionService.FetchForShirt(request));
    }

    public async Task<Response<ShirtOption?>> ShirtOptionFetch(Request<ShirtOption> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
            await ShirtOptionService.Fetch(request));
    }

    public async Task<Response<ShirtOption?>> ShirtOptionAdd(Request<ShirtOption> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            var response = await ShirtOptionService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Shirts, new ShirtOptionAdded
                {
                    Id = response.Value.Id,
                    ShirtId = request.Value.ShirtId,
                });

            return response;
        });
    }

    public async Task<Response> ShirtOptionSave(Request<ShirtOption> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            var response = await ShirtOptionService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Shirts, new ShirtOptionSaved
                {
                    Id = request.Value.Id,
                    ShirtId = request.Value.ShirtId,
                });

            return response;
        });
    }

    public async Task<Response> ShirtOptionRemove(Request<ShirtOption> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            var response = await ShirtOptionService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Shirts, new ShirtOptionRemoved
                {
                    Id = request.Value.Id,
                    ShirtId = request.Value.ShirtId,
                });

            return response;
        });
    }

    public async Task<Response> ShirtOptionSaveOrder(Request<IList<ShirtOption>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            var response = await ShirtOptionService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Shirts, new ShirtOptionsReordered
                {
                    ShirtId = request.Value.First().ShirtId,
                });

            return response;
        });
    }

    public async Task<Response> ShirtOptionSaveRelations(Request<ShirtOption> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
        {
            var response = await ShirtOptionService.SaveRelations(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Shirts, new ShirtOptionRelationsSaved
                {
                    Id = request.Value.Id,
                    ShirtId = request.Value.ShirtId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ShirtOptionFetchColorNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Shirts, async session =>
            await ShirtOptionService.FetchColorNames(request));
    }
}