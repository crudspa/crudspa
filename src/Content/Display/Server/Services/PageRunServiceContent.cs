namespace Crudspa.Content.Display.Server.Services;

public class PageRunServiceContent(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ISqlWrappers sqlWrappers) : IPageRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Page?>> Fetch(Request<Page> request) =>
        await wrappers.Try<Page?>(request, async _ => await PageRunSelectContent.Execute(Connection, request.Value, request.SessionId));

    public async Task<Response<Page?>> FetchForPane(Request<PagePane> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var pagePane = await PagePaneSelectForPane.Execute(Connection, request.SessionId, request.Value.PaneId);

            return pagePane?.PageId.HasValue == true
                ? await PageRunSelectContent.Execute(Connection, new() { Id = pagePane.PageId }, request.SessionId)
                : null;
        });
    }

    public async Task<Response> MarkViewed(Request<Page> request)
    {
        return await wrappers.Try(request, async response =>
        {
            if (request.Value.Id.HasNothing())
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PageRunMarkViewed.Execute(connection, transaction, request.Value, request.SessionId);
            });
        });
    }
}