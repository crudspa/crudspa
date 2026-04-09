using IPostService = Crudspa.Education.School.Shared.Contracts.Behavior.IPostService;
using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;
using PostSearch = Crudspa.Education.School.Shared.Contracts.Data.PostSearch;

namespace Crudspa.Education.School.Client.Services;

public class PostServiceTcp(IProxyWrappers proxyWrappers) : IPostService
{
    public async Task<Response<IList<Post>>> SearchForForum(Request<PostSearch> request) =>
        await proxyWrappers.Send<IList<Post>>("SchoolPostSearchForForum", request);

    public async Task<Response<IList<Post>>> FetchTreeForParent(Request<Post> request) =>
        await proxyWrappers.Send<IList<Post>>("SchoolPostFetchTreeForParent", request);

    public async Task<Response<Post?>> Fetch(Request<Post> request) =>
        await proxyWrappers.Send<Post?>("SchoolPostFetch", request);

    public async Task<Response<Post?>> Add(Request<Post> request) =>
        await proxyWrappers.Send<Post?>("SchoolPostAdd", request);

    public async Task<Response> Save(Request<Post> request) =>
        await proxyWrappers.Send("SchoolPostSave", request);

    public async Task<Response> Remove(Request<Post> request) =>
        await proxyWrappers.Send("SchoolPostRemove", request);

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("SchoolPostFetchGradeNames", request);

    public async Task<Response<IList<Orderable>>> FetchClassroomTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("SchoolPostFetchClassroomTypeNames", request);

    public async Task<Response> AddReaction(Request<PostReaction> request) =>
        await proxyWrappers.Send("SchoolPostAddReaction", request);
}