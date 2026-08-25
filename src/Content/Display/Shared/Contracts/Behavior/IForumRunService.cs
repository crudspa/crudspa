using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IForumRunService
{
    Task<Response<IList<Forum>>> FetchForums(Request request);
    Task<Response<Forum?>> FetchForum(Request<Forum> request);
    Task<Response<IList<Thread>>> SearchThreads(Request<ThreadSearch> request);
    Task<Response<Thread?>> FetchThread(Request<Thread> request);
    Task<Response<Thread?>> AddThread(Request<Thread> request);
    Task<Response> SaveThread(Request<Thread> request);
    Task<Response> RemoveThread(Request<Thread> request);
    Task<Response<IList<Comment>>> FetchComments(Request<Thread> request);
    Task<Response<Comment?>> AddComment(Request<Comment> request);
    Task<Response> SaveComment(Request<Comment> request);
    Task<Response> RemoveComment(Request<Comment> request);
    Task<Response> SetReaction(Request<CommentReaction> request);
}