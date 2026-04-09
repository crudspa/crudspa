using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;
using PostSearch = Crudspa.Education.School.Shared.Contracts.Data.PostSearch;

namespace Crudspa.Education.School.Shared.Contracts.Behavior;

public interface IPostService
{
    Task<Response<IList<Post>>> SearchForForum(Request<PostSearch> request);
    Task<Response<IList<Post>>> FetchTreeForParent(Request<Post> request);
    Task<Response<Post?>> Fetch(Request<Post> request);
    Task<Response<Post?>> Add(Request<Post> request);
    Task<Response> Save(Request<Post> request);
    Task<Response> Remove(Request<Post> request);
    Task<Response<IList<Orderable>>> FetchGradeNames(Request request);
    Task<Response<IList<Orderable>>> FetchClassroomTypeNames(Request request);
    Task<Response> AddReaction(Request<PostReaction> request);
}