namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<ElementType>>> SectionFetchElementTypes(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await SectionService.FetchElementTypes(request));
    }

    public async Task<Response<SectionElement?>> SectionCreateElement(Request<ElementSpec> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await SectionService.CreateElement(request));
    }
}