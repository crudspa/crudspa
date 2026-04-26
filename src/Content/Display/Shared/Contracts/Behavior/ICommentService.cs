
using Post = Crudspa.Content.Display.Shared.Contracts.Data.Post;

using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface ICommentService
{
    Task<Response<IList<Comment>>> FetchTreeForPost(Request<Post> request);
    Task<Response<IList<Comment>>> FetchTreeForThread(Request<Thread> request);
    Task<Response<Comment?>> Fetch(Request<Comment> request);
    Task<Response<Comment?>> Add(Request<Comment> request);
    Task<Response> Save(Request<Comment> request);
    Task<Response> Remove(Request<Comment> request);
}