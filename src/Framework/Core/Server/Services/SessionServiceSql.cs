namespace Crudspa.Framework.Core.Server.Services;

public class SessionServiceSql(
    IServiceWrappers serviceWrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ICryptographyService cryptographyService,
    ISessionFetcher sessionFetcher,
    IGatewayService gatewayService)
    : ISessionService
{
    private String Connection => configService.Fetch().Database;
    private Guid PortalId => configService.Fetch().PortalId;

    public async Task<Response<Session?>> FetchOrCreate(Request<Session> request)
    {
        return await serviceWrappers.Try<Session?>(request, async response =>
        {
            var session = request.Value;

            if (session.Id.HasSomething())
            {
                var selected = await sessionFetcher.Fetch(session.Id);

                if (selected is not null)
                    return selected;
            }

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var sessionId = cryptographyService.GetRandomGuid();

                await SessionInsert.Execute(connection, transaction, PortalId, sessionId);

                return await sessionFetcher.Fetch(sessionId);
            });
        });
    }

    public async Task<Response> IsValidForSignIn(Request<Session> request)
    {
        return await serviceWrappers.Try(request, async response =>
        {
            var session = request.Value;

            sessionFetcher.Invalidate(session.Id!.Value);

            var valid = await SessionIsValidForSignIn.Execute(Connection, session.Id, PortalId);

            if (!valid)
                response.AddError("Session is invalid for sign in.");
        });
    }

    public async Task InvalidateAll()
    {
        sessionFetcher.InvalidateAll();
        await gatewayService.Publish(new SessionsInvalidated());
    }

    public async Task<Response> End(Request request)
    {
        return await serviceWrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await SessionEnd.Execute(connection, transaction, request.SessionId));

            sessionFetcher.Invalidate(request.SessionId!.Value);
        });
    }
}