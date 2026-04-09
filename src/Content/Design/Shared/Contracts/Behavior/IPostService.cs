namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IPostService
{
    Task<Response<IList<Post>>> SearchForBlog(Request<PostSearch> request);
    Task<Response<Post?>> Fetch(Request<Post> request);
    Task<Response<Post?>> FetchPageId(Request<Post> request);
    Task<Response<Post?>> Add(Request<Post> request);
    Task<Response> Save(Request<Post> request);
    Task<Response> Remove(Request<Post> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Orderable>>> FetchPageTypeNames(Request request);
    Task<Response<Page?>> FetchPage(Request<PostPage> request);
    Task<Response> SavePage(Request<PostPage> request);
    Task<Response<IList<Section>>> FetchSections(Request<PostSection> request);
    Task<Response<Section?>> FetchSection(Request<PostSection> request);
    Task<Response<Section?>> AddSection(Request<PostSection> request);
    Task<Response> SaveSection(Request<PostSection> request);
    Task<Response> RemoveSection(Request<PostSection> request);
    Task<Response> SaveSectionOrder(Request<PostSection> request);
}