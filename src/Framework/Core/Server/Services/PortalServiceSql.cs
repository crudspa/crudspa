namespace Crudspa.Framework.Core.Server.Services;

public class PortalServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService)
    : IPortalService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Portal>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<Portal>>(request, async response =>
        {
            var portals = await PortalSelectAll.Execute(Connection, request.SessionId);
            return portals;
        });
    }

    public async Task<Response<Portal?>> Fetch(Request<Portal> request)
    {
        return await wrappers.Try<Portal?>(request, async response =>
        {
            var portal = await PortalSelect.Execute(Connection, request.SessionId, request.Value);
            return portal;
        });
    }

    public Task<Response> Save(Request<Portal> request)
    {
        return Task.FromResult<Response>(new());
    }
}