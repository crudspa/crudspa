using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<District>>> DistrictSearch(Request<DistrictSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await DistrictService.Search(request);
        });
    }

    public async Task<Response<District?>> DistrictFetch(Request<District> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await DistrictService.Fetch(request));
    }

    public async Task<Response<District?>> DistrictAdd(Request<District> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await DistrictService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new DistrictAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> DistrictSave(Request<District> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await DistrictService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new DistrictSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> DistrictRemove(Request<District> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await DistrictService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new DistrictRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> DistrictFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await DistrictService.FetchPermissionNames(request));
    }
}