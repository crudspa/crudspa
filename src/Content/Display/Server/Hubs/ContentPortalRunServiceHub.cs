namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<ContentPortal?>> ContentPortalRunFetch(Request<ContentPortal> request) =>
        await HubWrappers.AllowAnonymous(request, async () => await ContentPortalRunService.Fetch(request));
}