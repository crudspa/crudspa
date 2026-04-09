namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IBlogService
{
    Task<Response<IList<Blog>>> FetchForPortal(Request<Portal> request);
    Task<Response<Blog?>> Fetch(Request<Blog> request);
    Task<Response<Blog?>> Add(Request<Blog> request);
    Task<Response> Save(Request<Blog> request);
    Task<Response> Remove(Request<Blog> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
}