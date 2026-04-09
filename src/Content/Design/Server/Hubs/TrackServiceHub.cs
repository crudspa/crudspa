using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Track>>> TrackFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await TrackService.FetchForPortal(request));
    }

    public async Task<Response<Track?>> TrackFetch(Request<Track> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await TrackService.Fetch(request));
    }

    public async Task<Response<Track?>> TrackAdd(Request<Track> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await TrackService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new TrackAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> TrackSave(Request<Track> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await TrackService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new TrackSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> TrackRemove(Request<Track> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await TrackService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new TrackRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> TrackSaveOrder(Request<IList<Track>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
        {
            var response = await TrackService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Tracks, new TracksReordered
                {
                    PortalId = request.Value.First().PortalId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> TrackFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await TrackService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Named>>> TrackFetchAchievementNames(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
            await TrackService.FetchAchievementNames(request));
    }
}