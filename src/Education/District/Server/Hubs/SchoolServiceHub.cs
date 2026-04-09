using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.District.Server.Hubs;

public partial class DistrictHub
{
    public async Task<Response<IList<School>>> SchoolSearch(Request<SchoolSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SchoolService.Search(request);
        });
    }

    public async Task<Response<School?>> SchoolFetch(Request<School> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
            await SchoolService.Fetch(request));
    }

    public async Task<Response<School?>> SchoolAdd(Request<School> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            var response = await SchoolService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Schools, new SchoolAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> SchoolSave(Request<School> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            var response = await SchoolService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Schools, new SchoolSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> SchoolRemove(Request<School> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
        {
            var response = await SchoolService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Schools, new SchoolRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> SchoolFetchCommunityNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
            await SchoolService.FetchCommunityNames(request));
    }

    public async Task<Response<IList<Named>>> SchoolFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Schools, async session =>
            await SchoolService.FetchPermissionNames(request));
    }
}