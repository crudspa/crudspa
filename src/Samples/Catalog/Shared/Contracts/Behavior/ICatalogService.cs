namespace Crudspa.Samples.Catalog.Shared.Contracts.Behavior;

using Catalog = Data.Catalog;

public interface ICatalogService
{
    Task<Response<Catalog?>> Fetch(Request request);
    Task<Response> Save(Request<Catalog> request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
}