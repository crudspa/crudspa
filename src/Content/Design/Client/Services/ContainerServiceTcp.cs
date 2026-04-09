namespace Crudspa.Content.Design.Client.Services;

public class ContainerServiceTcp(IProxyWrappers proxyWrappers) : IContainerService
{
    public async Task<Response<IList<Orderable>>> FetchDirectionNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ContainerFetchDirectionNames", request);

    public async Task<Response<IList<Orderable>>> FetchWrapNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ContainerFetchWrapNames", request);

    public async Task<Response<IList<Orderable>>> FetchJustifyContentNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ContainerFetchJustifyContentNames", request);

    public async Task<Response<IList<Orderable>>> FetchAlignItemsNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ContainerFetchAlignItemsNames", request);

    public async Task<Response<IList<Orderable>>> FetchAlignContentNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ContainerFetchAlignContentNames", request);
}