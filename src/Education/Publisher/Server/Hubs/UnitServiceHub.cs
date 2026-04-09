using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Unit>>> UnitFetchAll(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitService.FetchAll(request));
    }

    public async Task<Response<Unit?>> UnitFetch(Request<Unit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitService.Fetch(request));
    }

    public async Task<Response<Unit?>> UnitAdd(Request<Unit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> UnitSave(Request<Unit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> UnitRemove(Request<Unit> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<Copy>> UnitCopy(Request<Copy> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitService.Copy(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitAdded
                {
                    Id = request.Value.NewId,
                });

            return response;
        });
    }

    public async Task<Response> UnitSaveOrder(Request<IList<Unit>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
        {
            var response = await UnitService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Units, new UnitsReordered());

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> UnitFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Orderable>>> UnitFetchGradeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitService.FetchGradeNames(request));
    }

    public async Task<Response<IList<Orderable>>> UnitFetchUnitNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitService.FetchUnitNames(request));
    }

    public async Task<Response<IList<Named>>> UnitFetchAchievementNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Units, async session =>
            await UnitService.FetchAchievementNames(request));
    }
}