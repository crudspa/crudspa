namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Orderable>>> ItemFetchBasisNames(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ItemService.FetchBasisNames(request));
    }

    public async Task<Response<IList<Orderable>>> ItemFetchAlignSelfNames(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ItemService.FetchAlignSelfNames(request));
    }
}