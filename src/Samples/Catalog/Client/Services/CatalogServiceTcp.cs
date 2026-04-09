namespace Crudspa.Samples.Catalog.Client.Services;

using Catalog = Shared.Contracts.Data.Catalog;

public class CatalogServiceTcp(IProxyWrappers proxyWrappers) : ICatalogService
{
    public async Task<Response<Catalog?>> Fetch(Request request) =>
        await proxyWrappers.Send<Catalog?>("CatalogFetch", request);

    public async Task<Response> Save(Request<Catalog> request) =>
        await proxyWrappers.Send("CatalogSave", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("CatalogFetchPermissionNames", request);
}