namespace Crudspa.Samples.Catalog.Client.Services;

public class ShirtServiceTcp(IProxyWrappers proxyWrappers) : IShirtService
{
    public async Task<Response<IList<Shirt>>> Search(Request<ShirtSearch> request) =>
        await proxyWrappers.Send<IList<Shirt>>("ShirtSearch", request);

    public async Task<Response<Shirt?>> Fetch(Request<Shirt> request) =>
        await proxyWrappers.Send<Shirt?>("ShirtFetch", request);

    public async Task<Response<Shirt?>> Add(Request<Shirt> request) =>
        await proxyWrappers.Send<Shirt?>("ShirtAdd", request);

    public async Task<Response> Save(Request<Shirt> request) =>
        await proxyWrappers.Send("ShirtSave", request);

    public async Task<Response> Remove(Request<Shirt> request) =>
        await proxyWrappers.Send("ShirtRemove", request);

    public async Task<Response<IList<Orderable>>> FetchBrandNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ShirtFetchBrandNames", request);
}