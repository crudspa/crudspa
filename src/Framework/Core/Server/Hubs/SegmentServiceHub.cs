using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<IList<Segment>>> SegmentFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchForPortal(request));
    }

    public async Task<Response<IList<Segment>>> SegmentFetchForParent(Request<Segment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchForParent(request));
    }

    public async Task<Response<Segment?>> SegmentFetch(Request<Segment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.Fetch(request));
    }

    public async Task<Response<Segment?>> SegmentAdd(Request<Segment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await SegmentService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new SegmentAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                    ParentId = request.Value.ParentId,
                });

            return response;
        });
    }

    public async Task<Response> SegmentSave(Request<Segment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await SegmentService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new SegmentSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                    ParentId = request.Value.ParentId,
                });

            return response;
        });
    }

    public async Task<Response> SegmentRemove(Request<Segment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await SegmentService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new SegmentRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                    ParentId = request.Value.ParentId,
                });

            return response;
        });
    }

    public async Task<Response> SegmentSaveOrder(Request<IList<Segment>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await SegmentService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new SegmentsReordered
                {
                    PortalId = request.Value.First().PortalId,
                    ParentId = request.Value.First().ParentId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> SegmentFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Named>>> SegmentFetchPermissionNames(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchPermissionNames(request));
    }

    public async Task<Response<IList<IconFull>>> SegmentFetchIcons(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchIcons(request));
    }

    public async Task<Response<IList<SegmentTypeFull>>> SegmentFetchSegmentTypes(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchSegmentTypes(request));
    }

    public async Task<Response<IList<Named>>> SegmentFetchLicenseNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchLicenseNames(request));
    }

    public async Task<Response<Segment?>> SegmentFetchStructure(Request<Segment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchStructure(request));
    }

    public async Task<Response> SegmentSaveStructure(Request<Segment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var response = await SegmentService.SaveStructure(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Segments, new SegmentSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                    ParentId = request.Value.ParentId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Expandable>>> SegmentFetchTree(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
            await SegmentService.FetchTree(request));
    }

    public async Task<Response> SegmentMove(Request<Segment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Segments, async session =>
        {
            var fetchResponse = await SegmentService.Fetch(request);

            if (!fetchResponse.Ok)
                return new() { Errors = fetchResponse.Errors };

            if (fetchResponse.Value is null)
                return new();

            var existing = fetchResponse.Value;

            var response = await SegmentService.Move(request);

            if (response.Ok)
            {
                await Notify(request.SessionId, PermissionIds.Segments, new SegmentMoved
                {
                    Id = existing.Id,
                    PortalId = request.Value.PortalId,
                    ParentId = request.Value.ParentId,
                    OldPortalId = existing.PortalId,
                    OldParentId = existing.ParentId,
                    NewPortalId = request.Value.PortalId,
                    NewParentId = request.Value.ParentId,
                });
            }

            return response;
        });
    }
}