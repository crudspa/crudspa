namespace Crudspa.Samples.Catalog.Shared.Contracts.Behavior;

public interface IShirtOptionService
{
    Task<Response<IList<ShirtOption>>> FetchForShirt(Request<Shirt> request);
    Task<Response<ShirtOption?>> Fetch(Request<ShirtOption> request);
    Task<Response<ShirtOption?>> Add(Request<ShirtOption> request);
    Task<Response> Save(Request<ShirtOption> request);
    Task<Response> Remove(Request<ShirtOption> request);
    Task<Response> SaveRelations(Request<ShirtOption> request);
    Task<Response> SaveOrder(Request<IList<ShirtOption>> request);
    Task<Response<IList<Orderable>>> FetchColorNames(Request request);
}