namespace Crudspa.Samples.Catalog.Client.Services;

public class CatalogContactServiceTcp(IProxyWrappers proxyWrappers) : ICatalogContactService
{
    public async Task<Response<IList<CatalogContact>>> Search(Request<CatalogContactSearch> request) =>
        await proxyWrappers.Send<IList<CatalogContact>>("CatalogContactSearch", request);

    public async Task<Response<CatalogContact?>> Fetch(Request<CatalogContact> request) =>
        await proxyWrappers.Send<CatalogContact?>("CatalogContactFetch", request);

    public async Task<Response<CatalogContact?>> Add(Request<CatalogContact> request) =>
        await proxyWrappers.Send<CatalogContact?>("CatalogContactAdd", request);

    public async Task<Response> Save(Request<CatalogContact> request) =>
        await proxyWrappers.Send("CatalogContactSave", request);

    public async Task<Response> Remove(Request<CatalogContact> request) =>
        await proxyWrappers.Send("CatalogContactRemove", request);

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("CatalogContactFetchRoleNames", request);

    public async Task<Response> SendAccessCode(Request<CatalogContact> request) =>
        await proxyWrappers.Send("CatalogContactSendAccessCode", request);
}