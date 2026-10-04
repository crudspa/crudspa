
namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<IList<PublicationSummary>>> PublicationRunFetchAll(Request request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationRunService service)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await service.FetchAll(request));
    }

    public async Task<Response<IList<Orderable>>> PublicationRunFetchPublicationResourceTypeNames(Request request, [Microsoft.AspNetCore.Mvc.FromServices] IPublicationRunService service)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await service.FetchPublicationResourceTypeNames(request));
    }
}