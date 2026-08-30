namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<Session?>> SessionStateFetch(Request<Session> request)
    {
        return await HubWrappers.AllowAnonymous(request, async () =>
        {
            var authenticatedSessionId = Context.User is { } principal ? principal.ReadAuthenticatedSessionId() : null;

            if (authenticatedSessionId is not null)
                request.Value.Id = authenticatedSessionId;

            var response = await SessionStateService.Fetch(request);

            if (response.Value is not null && authenticatedSessionId is not null)
                response.Value.ServerAuthenticated = true;

            return response;
        });
    }
}