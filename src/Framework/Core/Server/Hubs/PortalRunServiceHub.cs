namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<Portal?>> PortalRunFetch(Request<Portal> request) =>
        await HubWrappers.AllowAnonymous(request, async () => await PortalRunService.Fetch(request));
}