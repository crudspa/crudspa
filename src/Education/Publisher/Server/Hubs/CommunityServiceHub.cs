using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Community>>> CommunityFetchForDistrict(Request<District> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await CommunityService.FetchForDistrict(request));
    }

    public async Task<Response<Community?>> CommunityFetch(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await CommunityService.Fetch(request));
    }

    public async Task<Response<Community?>> CommunityAdd(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await CommunityService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new CommunityAdded
                {
                    Id = response.Value.Id,
                    DistrictId = request.Value.DistrictId,
                });

            return response;
        });
    }

    public async Task<Response> CommunitySave(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await CommunityService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new CommunitySaved
                {
                    Id = request.Value.Id,
                    DistrictId = request.Value.DistrictId,
                });

            return response;
        });
    }

    public async Task<Response> CommunityRemove(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await CommunityService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new CommunityRemoved
                {
                    Id = request.Value.Id,
                    DistrictId = request.Value.DistrictId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Selectable>>> CommunityFetchDistrictContacts(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await CommunityService.FetchDistrictContacts(request));
    }

    public async Task<Response<Community?>> CommunityFetchSchoolSelections(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
            await CommunityService.FetchSchoolSelections(request));
    }

    public async Task<Response> CommunitySaveSchoolSelections(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Districts, async session =>
        {
            var response = await CommunityService.SaveSchoolSelections(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Districts, new CommunityRelationsSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }
}