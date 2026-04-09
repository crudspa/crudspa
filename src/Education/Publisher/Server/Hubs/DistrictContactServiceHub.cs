using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<DistrictContact>>> DistrictContactSearch(Request<DistrictContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await DistrictContactService.Search(request);
        });
    }

    public async Task<Response<IList<DistrictContact>>> DistrictContactSearchForDistrict(Request<DistrictContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await DistrictContactService.SearchForDistrict(request);
        });
    }

    public async Task<Response<DistrictContact?>> DistrictContactFetch(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await DistrictContactService.Fetch(request));
    }

    public async Task<Response<DistrictContact?>> DistrictContactAdd(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await DistrictContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new DistrictContactAdded
                {
                    Id = response.Value.Id,
                    DistrictId = request.Value.DistrictId,
                });

            return response;
        });
    }

    public async Task<Response> DistrictContactSave(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await DistrictContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new DistrictContactSaved
                {
                    Id = request.Value.Id,
                    DistrictId = request.Value.DistrictId,
                });

            return response;
        });
    }

    public async Task<Response> DistrictContactRemove(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await DistrictContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new DistrictContactRemoved
                {
                    Id = request.Value.Id,
                    DistrictId = request.Value.DistrictId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> DistrictContactFetchDistrictNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await DistrictContactService.FetchDistrictNames(request));
    }

    public async Task<Response<IList<Named>>> DistrictContactFetchRoleNames(Request<District> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await DistrictContactService.FetchRoleNames(request));
    }

    public async Task<Response> DistrictContactSendAccessCode(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await DistrictContactService.SendAccessCode(request));
    }
}