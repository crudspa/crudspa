using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.District.Server.Hubs;

public partial class DistrictHub
{
    public async Task<Response<IList<Community>>> CommunityFetchAll(Request<Shared.Contracts.Data.District> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Communities, async session =>
            await CommunityService.FetchAll(request));
    }

    public async Task<Response<Community?>> CommunityFetch(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Communities, async session =>
            await CommunityService.Fetch(request));
    }

    public async Task<Response<Community?>> CommunityAdd(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Communities, async session =>
        {
            var response = await CommunityService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Communities, new CommunityAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> CommunitySave(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Communities, async session =>
        {
            var response = await CommunityService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Communities, new CommunitySaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> CommunityRemove(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Communities, async session =>
        {
            var response = await CommunityService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Communities, new CommunityRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Selectable>>> CommunityFetchDistrictContacts(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Communities, async session =>
            await CommunityService.FetchDistrictContacts(request));
    }

    public async Task<Response<Community?>> CommunityFetchSchoolSelections(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Communities, async session =>
            await CommunityService.FetchSchoolSelections(request));
    }

    public async Task<Response> CommunitySaveSchoolSelections(Request<Community> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Communities, async session =>
        {
            var response = await CommunityService.SaveSchoolSelections(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Communities, new CommunityRelationsSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }
}