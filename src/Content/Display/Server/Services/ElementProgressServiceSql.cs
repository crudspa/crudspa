namespace Crudspa.Content.Display.Server.Services;

public class ElementProgressServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IElementProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ElementProgress>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<ElementProgress>>(request, async response =>
            await ElementProgressSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<ElementProgress> Fetch(Request<Element> request)
    {
        return await ElementProgressSelect.Execute(Connection, request.SessionId, request.Value.Id);
    }

    public async Task<Response> AddCompleted(Request<ElementCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ElementCompletedInsert.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response> AddLink(Request<ElementLink> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var elementLink = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ElementLinkFollowedInsert.Execute(connection, transaction, request.SessionId, elementLink);
            });
        });
    }
}