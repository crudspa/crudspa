using IPostService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IPostService;
using Post = Crudspa.Education.Publisher.Shared.Contracts.Data.Post;
using PostSearch = Crudspa.Education.Publisher.Shared.Contracts.Data.PostSearch;

namespace Crudspa.Education.Publisher.Client.Services;

public class PostServiceTcp(IProxyWrappers proxyWrappers) : IPostService
{
    public async Task<Response<IList<Post>>> SearchForForum(Request<PostSearch> request) =>
        await proxyWrappers.Send<IList<Post>>("PublisherPostSearchForForum", request);

    public async Task<Response<IList<Post>>> FetchTreeForParent(Request<Post> request) =>
        await proxyWrappers.Send<IList<Post>>("PublisherPostFetchTreeForParent", request);

    public async Task<Response<Post?>> Fetch(Request<Post> request) =>
        await proxyWrappers.Send<Post?>("PublisherPostFetch", request);

    public async Task<Response<Post?>> Add(Request<Post> request) =>
        await proxyWrappers.Send<Post?>("PublisherPostAdd", request);

    public async Task<Response> Save(Request<Post> request) =>
        await proxyWrappers.Send("PublisherPostSave", request);

    public async Task<Response> Remove(Request<Post> request) =>
        await proxyWrappers.Send("PublisherPostRemove", request);

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("PublisherPostFetchGradeNames", request);

    public async Task<Response<IList<Orderable>>> FetchClassroomTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("PublisherPostFetchClassroomTypeNames", request);

    public async Task<Response> AddReaction(Request<PostReaction> request) =>
        await proxyWrappers.Send("PublisherPostAddReaction", request);
}