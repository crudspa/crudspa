using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<Pane?>> PaneFetch(Request<Pane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await PaneService.Fetch(request));
    }

    public async Task<Response> PaneSave(Request<Pane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PaneService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new PaneSaved
                {
                    Id = request.Value.Id,
                    SegmentId = request.Value.SegmentId,
                });

            return response;
        });
    }

    public async Task<Response> PaneRemove(Request<Pane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PaneService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new PaneRemoved
                {
                    Id = request.Value.Id,
                    SegmentId = request.Value.SegmentId,
                });

            return response;
        });
    }

    public async Task<Response> PaneSaveOrder(Request<IList<Pane>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await PaneService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new PanesReordered
                {
                    SegmentId = request.Value.First().SegmentId,
                });

            return response;
        });
    }

    public async Task<Response<IList<PaneTypeFull>>> PaneFetchPaneTypes(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await PaneService.FetchPaneTypes(request));
    }

    public async Task<Response> PaneMove(Request<Pane> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var existingResponse = await PaneService.Fetch(new(request.SessionId, new() { Id = request.Value.Id }));

            if (!existingResponse.Ok)
                return new() { Errors = existingResponse.Errors };

            if (existingResponse.Value is null)
                return new();

            var fromSegmentId = existingResponse.Value.SegmentId;
            var toSegmentId = request.Value.SegmentId;

            var response = await PaneService.Move(request);

            if (!response.Ok)
                return response;

            if (toSegmentId.HasValue && !toSegmentId.Equals(fromSegmentId))
                await Notify(request.SessionId, PermissionIds.Segments, new PaneMoved
                {
                    Id = request.Value.Id,
                    SegmentId = toSegmentId,
                    OldSegmentId = fromSegmentId,
                    NewSegmentId = toSegmentId,
                });

            return response;
        });
    }
}