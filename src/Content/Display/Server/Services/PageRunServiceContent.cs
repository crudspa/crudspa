namespace Crudspa.Content.Display.Server.Services;

public class PageRunServiceContent(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ISqlWrappers sqlWrappers) : IPageRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Page?>> Fetch(Request<Page> request) =>
        await wrappers.Try<Page?>(request, async _ => await PageRunSelectContent.Execute(Connection, request.Value, request.SessionId));

    public async Task<Response> MarkViewed(Request<Page> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PageRunMarkViewed.Execute(connection, transaction, request.Value, request.SessionId);
            });
        });
    }
}