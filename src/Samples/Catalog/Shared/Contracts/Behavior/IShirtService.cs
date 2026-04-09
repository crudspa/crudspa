namespace Crudspa.Samples.Catalog.Shared.Contracts.Behavior;

public interface IShirtService
{
    Task<Response<IList<Shirt>>> Search(Request<ShirtSearch> request);
    Task<Response<Shirt?>> Fetch(Request<Shirt> request);
    Task<Response<Shirt?>> Add(Request<Shirt> request);
    Task<Response> Save(Request<Shirt> request);
    Task<Response> Remove(Request<Shirt> request);
    Task<Response<IList<Orderable>>> FetchBrandNames(Request request);
}