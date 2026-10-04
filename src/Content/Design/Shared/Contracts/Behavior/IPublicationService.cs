
namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IPublicationService
{
    Task<Response<IList<Publication>>> FetchForPortal(Request<Portal> request);
    Task<Response<Publication?>> Fetch(Request<Publication> request);
    Task<Response<Publication?>> Add(Request<Publication> request);
    Task<Response> Save(Request<Publication> request);
    Task<Response> Remove(Request<Publication> request);
    Task<Response> SaveOrder(Request<IList<Publication>> request);
    Task<Response<IList<Orderable>>> FetchPublicationGroupNames(Request<Portal> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Orderable>>> FetchPublicationResourceTypeNames(Request request);
}