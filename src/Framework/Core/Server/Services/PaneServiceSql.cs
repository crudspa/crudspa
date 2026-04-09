namespace Crudspa.Framework.Core.Server.Services;

public class PaneServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISessionService sessionService)
    : IPaneService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Pane?>> Fetch(Request<Pane> request)
    {
        return await wrappers.Try<Pane?>(request, async response =>
        {
            var pane = await PaneSelect.Execute(Connection, request.SessionId, request.Value);

            return pane;
        });
    }

    public async Task<Response> Save(Request<Pane> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var pane = request.Value;
            var existing = await PaneSelect.Execute(Connection, request.SessionId, pane);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PaneUpdateType.Execute(connection, transaction, request.SessionId, pane);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> Remove(Request<Pane> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pane = request.Value;
            var existing = await PaneSelect.Execute(Connection, request.SessionId, pane);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PaneDelete.Execute(connection, transaction, request.SessionId, pane);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Pane>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var panes = request.Value;

            panes.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PaneUpdateOrdinals.Execute(connection, transaction, request.SessionId, panes);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response<IList<PaneTypeFull>>> FetchPaneTypes(Request<Portal> request)
    {
        return await wrappers.Try<IList<PaneTypeFull>>(request, async response =>
            await PaneTypeSelectFull.Execute(Connection, request.Value.Id));
    }

    public async Task<Response> Move(Request<Pane> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pane = request.Value;

            var existing = await PaneSelect.Execute(Connection, request.SessionId, pane);
            var destinationSegment = await SegmentSelect.Execute(Connection, request.SessionId, new() { Id = pane.SegmentId });

            if (existing is null)
                return;

            if (destinationSegment is null)
            {
                response.AddError("Destination segment was not found.");
                return;
            }

            var destinationPaneTypes = await PaneTypeSelectFull.Execute(Connection, destinationSegment.PortalId);

            if (!destinationPaneTypes.Any(x => x.Id.Equals(existing.TypeId)))
            {
                response.AddError("Pane type is not available in the destination portal.");
                return;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PaneUpdateSegment.Execute(connection, transaction, request.SessionId, pane);
            });

            await sessionService.InvalidateAll();
        });
    }
}