namespace Crudspa.Framework.Core.Server.Services;

public class SessionFetcherSql(IServerConfigService configService) : ISessionFetcher
{
    private String Connection => configService.Fetch().Database;
    private Guid PortalId => configService.Fetch().PortalId;

    public async Task<Session?> Fetch(Guid? sessionId)
    {
        if (sessionId is null)
            return null;

        return await SessionSelect.Execute(Connection, sessionId, PortalId);
    }

    public void Invalidate(Guid? sessionId) { }

    public void InvalidateAll() { }
}