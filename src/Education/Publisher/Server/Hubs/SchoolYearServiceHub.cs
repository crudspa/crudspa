using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<SchoolYear>>> SchoolYearFetchAll(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Years, async session =>
            await SchoolYearService.FetchAll(request));
    }

    public async Task<Response<SchoolYear?>> SchoolYearFetch(Request<SchoolYear> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Years, async session =>
            await SchoolYearService.Fetch(request));
    }

    public async Task<Response<SchoolYear?>> SchoolYearAdd(Request<SchoolYear> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Years, async session =>
        {
            var response = await SchoolYearService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Years, new SchoolYearAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> SchoolYearSave(Request<SchoolYear> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Years, async session =>
        {
            var response = await SchoolYearService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Years, new SchoolYearSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> SchoolYearRemove(Request<SchoolYear> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Years, async session =>
        {
            var response = await SchoolYearService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Years, new SchoolYearRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }
}