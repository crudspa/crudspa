namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<Session?>> SessionStateFetch(Request<Session> request)
    {
        return await HubWrappers.AllowAnonymous(request, async () =>
            await SessionStateService.Fetch(request));
    }
}