using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Achievement>>> AchievementFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
            await AchievementService.FetchForPortal(request));
    }

    public async Task<Response<Achievement?>> AchievementFetch(Request<Achievement> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
            await AchievementService.Fetch(request));
    }

    public async Task<Response<Achievement?>> AchievementAdd(Request<Achievement> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
        {
            var response = await AchievementService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Achievements, new AchievementAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> AchievementSave(Request<Achievement> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
        {
            var response = await AchievementService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Achievements, new AchievementSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> AchievementRemove(Request<Achievement> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
        {
            var response = await AchievementService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Achievements, new AchievementRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }
}