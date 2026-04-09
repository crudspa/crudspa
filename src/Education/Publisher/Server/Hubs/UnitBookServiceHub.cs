using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<UnitBook>>> UnitBookFetchForUnit(Request<Unit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitBookService.FetchForUnit(request));
    }

    public async Task<Response<UnitBook?>> UnitBookFetch(Request<UnitBook> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitBookService.Fetch(request));
    }

    public async Task<Response<UnitBook?>> UnitBookAdd(Request<UnitBook> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitBookService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitBookAdded
                {
                    Id = response.Value.Id,
                    UnitId = request.Value.UnitId,
                });

            return response;
        });
    }

    public async Task<Response> UnitBookSave(Request<UnitBook> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitBookService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitBookSaved
                {
                    Id = request.Value.Id,
                    UnitId = request.Value.UnitId,
                });

            return response;
        });
    }

    public async Task<Response> UnitBookRemove(Request<UnitBook> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitBookService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitBookRemoved
                {
                    Id = request.Value.Id,
                    UnitId = request.Value.UnitId,
                });

            return response;
        });
    }

    public async Task<Response> UnitBookSaveOrder(Request<IList<UnitBook>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitBookService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitBooksReordered
                {
                    UnitId = request.Value.First().UnitId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> UnitBookFetchBookNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitBookService.FetchBookNames(request));
    }
}