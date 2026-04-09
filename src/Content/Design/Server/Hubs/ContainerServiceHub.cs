namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Orderable>>> ContainerFetchDirectionNames(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ContainerService.FetchDirectionNames(request));
    }

    public async Task<Response<IList<Orderable>>> ContainerFetchWrapNames(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ContainerService.FetchWrapNames(request));
    }

    public async Task<Response<IList<Orderable>>> ContainerFetchJustifyContentNames(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ContainerService.FetchJustifyContentNames(request));
    }

    public async Task<Response<IList<Orderable>>> ContainerFetchAlignItemsNames(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ContainerService.FetchAlignItemsNames(request));
    }

    public async Task<Response<IList<Orderable>>> ContainerFetchAlignContentNames(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await ContainerService.FetchAlignContentNames(request));
    }
}