namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IForumService
{
    Task<Response<IList<Forum>>> FetchForPortal(Request<Portal> request);
    Task<Response<Forum?>> Fetch(Request<Forum> request);
    Task<Response<Forum?>> Add(Request<Forum> request);
    Task<Response> Save(Request<Forum> request);
    Task<Response> Remove(Request<Forum> request);
    Task<Response> SaveOrder(Request<IList<Forum>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
}