namespace Crudspa.Content.Design.Client.Services;

public class ItemServiceTcp(IProxyWrappers proxyWrappers) : IItemService
{
    public async Task<Response<IList<Orderable>>> FetchBasisNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ItemFetchBasisNames", request);

    public async Task<Response<IList<Orderable>>> FetchAlignSelfNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ItemFetchAlignSelfNames", request);
}