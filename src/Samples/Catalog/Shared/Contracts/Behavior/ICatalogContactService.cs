namespace Crudspa.Samples.Catalog.Shared.Contracts.Behavior;

public interface ICatalogContactService
{
    Task<Response<IList<CatalogContact>>> Search(Request<CatalogContactSearch> request);
    Task<Response<CatalogContact?>> Fetch(Request<CatalogContact> request);
    Task<Response<CatalogContact?>> Add(Request<CatalogContact> request);
    Task<Response> Save(Request<CatalogContact> request);
    Task<Response> Remove(Request<CatalogContact> request);
    Task<Response<IList<Named>>> FetchRoleNames(Request request);
    Task<Response> SendAccessCode(Request<CatalogContact> request);
}