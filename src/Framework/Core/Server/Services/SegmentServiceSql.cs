namespace Crudspa.Framework.Core.Server.Services;

public class SegmentServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISessionService sessionService)
    : ISegmentService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Segment>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Segment>>(request, async response =>
        {
            var segments = await SegmentSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return segments;
        });
    }

    public async Task<Response<IList<Segment>>> FetchForParent(Request<Segment> request)
    {
        return await wrappers.Try<IList<Segment>>(request, async response =>
        {
            var segments = await SegmentSelectForParent.Execute(Connection, request.SessionId, request.Value.Id);

            return segments;
        });
    }

    public async Task<Response<Segment?>> Fetch(Request<Segment> request)
    {
        return await wrappers.Try<Segment?>(request, async response =>
        {
            var segment = await SegmentSelect.Execute(Connection, request.SessionId, request.Value);

            return segment;
        });
    }

    public async Task<Response<Segment?>> Add(Request<Segment> request)
    {
        return await wrappers.Validate<Segment?, Segment>(request, async response =>
        {
            var segment = request.Value;

            if (segment.ParentId is not null)
            {
                var parent = await SegmentSelect.Execute(Connection, request.SessionId, new() { Id = segment.ParentId });

                if (parent is null) return null;

                segment.PortalId = parent.PortalId;
            }

            var unique = await SegmentKeyIsUnique.Execute(Connection, segment);

            if (!unique)
            {
                response.AddError("Key must be unique among its siblings.");
                return null;
            }

            if (!segment.Panes.HasItems())
            {
                var addedSegment = await sqlWrappers.WithConnection(async (connection, transaction) =>
                {
                    var id = await SegmentInsert.Execute(connection, transaction, request.SessionId, segment);

                    return new Segment
                    {
                        Id = id,
                        PortalId = segment.PortalId,
                        ParentId = segment.ParentId,
                    };
                });

                await sessionService.InvalidateAll();

                return addedSegment;
            }

            var structuredSegment = await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await SegmentInsert.Execute(connection, transaction, request.SessionId, segment);
                segment.Id = id;

                foreach (var pane in segment.Panes)
                    pane.SegmentId = id;

                await PersistStructure(connection, transaction, request.SessionId, segment, []);

                return new Segment
                {
                    Id = id,
                    PortalId = segment.PortalId,
                    ParentId = segment.ParentId,
                };
            });

            await sessionService.InvalidateAll();

            return structuredSegment;
        });
    }

    public async Task<Response> Save(Request<Segment> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var segment = request.Value;
            var existing = await SegmentSelect.Execute(Connection, request.SessionId, segment);

            if (existing is null)
                return;

            var unique = await SegmentKeyIsUnique.Execute(Connection, segment);

            if (!unique)
            {
                response.AddError("Key must be unique among its siblings.");
                return;
            }

            if (!segment.Panes.HasItems())
            {
                await sqlWrappers.WithConnection(async (connection, transaction) =>
                {
                    await SegmentUpdate.Execute(connection, transaction, request.SessionId, segment);
                });

                await sessionService.InvalidateAll();

                return;
            }

            var existingStructure = await SegmentSelectStructure.Execute(Connection, request.SessionId, segment);

            if (existingStructure is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await SegmentUpdate.Execute(connection, transaction, request.SessionId, segment);
                await PersistStructure(connection, transaction, request.SessionId, segment, existingStructure.Panes);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> Remove(Request<Segment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var segment = request.Value;
            var existing = await SegmentSelect.Execute(Connection, request.SessionId, segment);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SegmentDelete.Execute(connection, transaction, request.SessionId, segment);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Segment>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var segments = request.Value;

            segments.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SegmentUpdateOrdinals.Execute(connection, transaction, request.SessionId, segments);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request<Portal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, request.Value.Id));
    }

    public async Task<Response<IList<IconFull>>> FetchIcons(Request request)
    {
        return await wrappers.Try<IList<IconFull>>(request, async response =>
            await IconSelectFull.Execute(Connection));
    }

    public async Task<Response<IList<SegmentTypeFull>>> FetchSegmentTypes(Request<Portal> request)
    {
        return await wrappers.Try<IList<SegmentTypeFull>>(request, async response =>
            await SegmentTypeSelectFull.Execute(Connection, request.Value.Id));
    }

    public async Task<Response<IList<Named>>> FetchLicenseNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await LicenseSelectNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response<Segment?>> FetchStructure(Request<Segment> request)
    {
        return await wrappers.Try<Segment?>(request, async response =>
        {
            var segment = await SegmentSelectStructure.Execute(Connection, request.SessionId, request.Value);
            return segment;
        });
    }

    public async Task<Response> SaveStructure(Request<Segment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var segment = request.Value;

            if (!segment.Panes.HasItems())
            {
                response.AddError("At least one pane is required.");
                return;
            }

            var existing = await SegmentSelectStructure.Execute(Connection, request.SessionId, segment);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await PersistStructure(connection, transaction, request.SessionId, segment, existing.Panes);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response<IList<Expandable>>> FetchTree(Request request)
    {
        return await wrappers.Try<IList<Expandable>>(request, async response =>
        {
            var portals = await SegmentSelectTree.Execute(Connection, request.SessionId);
            return portals;
        });
    }

    public async Task<Response> Move(Request<Segment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var segment = request.Value;

            if (segment.ParentId.Equals(segment.Id))
            {
                response.AddError("Segment cannot be a parent of itself.");
                return;
            }

            var existing = await SegmentSelect.Execute(Connection, request.SessionId, segment);

            if (existing is null) return;

            if (segment.ParentId is not null)
            {
                var parent = await SegmentSelect.Execute(Connection, request.SessionId, new() { Id = segment.ParentId });

                if (parent is null) return;

                segment.PortalId = parent.PortalId;

                var visited = new HashSet<Guid>();
                var ancestor = parent;

                while (true)
                {
                    if (segment.Id.HasValue && ancestor.Id.HasValue && ancestor.Id.Value.Equals(segment.Id.Value))
                    {
                        response.AddError("Segment cannot be moved to one of its descendants.");
                        return;
                    }

                    if (ancestor.Id.HasValue && !visited.Add(ancestor.Id.Value))
                    {
                        response.AddError("Destination segment hierarchy contains a cycle.");
                        return;
                    }

                    if (ancestor.ParentId is null)
                        break;

                    ancestor = await SegmentSelect.Execute(Connection, request.SessionId, new() { Id = ancestor.ParentId });

                    if (ancestor is null)
                        return;
                }
            }
            else
            {
                var destinationPortal = await PortalSelect.Execute(Connection, request.SessionId, new() { Id = segment.PortalId });

                if (destinationPortal is null)
                    return;
            }

            existing.PortalId = segment.PortalId;
            existing.ParentId = segment.ParentId;

            var unique = await SegmentKeyIsUnique.Execute(Connection, existing);

            if (!unique)
            {
                response.AddError("Key must be unique among its siblings.");
                return;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SegmentUpdateParent.Execute(connection, transaction, request.SessionId, segment);
            });

            await sessionService.InvalidateAll();
        });
    }

    private static async Task PersistStructure(SqlConnection connection, SqlTransaction transaction, Guid? sessionId, Segment segment, IEnumerable<Pane>? existingPanes)
    {
        segment.Panes.EnsureOrder();

        await SegmentUpdateType.Execute(connection, transaction, sessionId, segment);

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existingPanes,
            segment.Panes,
            PaneInsert.Execute,
            PaneUpdate.Execute,
            PaneDelete.Execute);
    }
}