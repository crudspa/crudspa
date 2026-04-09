namespace Crudspa.Samples.Catalog.Client.Services;

public class ShirtOptionServiceTcp(IProxyWrappers proxyWrappers) : IShirtOptionService
{
    public async Task<Response<IList<ShirtOption>>> FetchForShirt(Request<Shirt> request) =>
        await proxyWrappers.Send<IList<ShirtOption>>("ShirtOptionFetchForShirt", request);

    public async Task<Response<ShirtOption?>> Fetch(Request<ShirtOption> request) =>
        await proxyWrappers.Send<ShirtOption?>("ShirtOptionFetch", request);

    public async Task<Response<ShirtOption?>> Add(Request<ShirtOption> request) =>
        await proxyWrappers.Send<ShirtOption?>("ShirtOptionAdd", request);

    public async Task<Response> Save(Request<ShirtOption> request) =>
        await proxyWrappers.Send("ShirtOptionSave", request);

    public async Task<Response> Remove(Request<ShirtOption> request) =>
        await proxyWrappers.Send("ShirtOptionRemove", request);

    public async Task<Response> SaveRelations(Request<ShirtOption> request) =>
        await proxyWrappers.Send("ShirtOptionSaveRelations", request);

    public async Task<Response> SaveOrder(Request<IList<ShirtOption>> request) =>
        await proxyWrappers.Send("ShirtOptionSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchColorNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ShirtOptionFetchColorNames", request);
}