using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Client.Services;

public class ForumRunServiceTcp(IProxyWrappers proxyWrappers) : IForumRunService
{
    public async Task<Response<IList<Forum>>> FetchForums(Request request) =>
        await proxyWrappers.Send<IList<Forum>>("ForumRunFetchForums", request);

    public async Task<Response<Forum?>> FetchForum(Request<Forum> request) =>
        await proxyWrappers.Send<Forum?>("ForumRunFetchForum", request);

    public async Task<Response<IList<Thread>>> SearchThreads(Request<ThreadSearch> request) =>
        await proxyWrappers.Send<IList<Thread>>("ForumRunSearchThreads", request);

    public async Task<Response<Thread?>> FetchThread(Request<Thread> request) =>
        await proxyWrappers.Send<Thread?>("ForumRunFetchThread", request);

    public async Task<Response<Thread?>> AddThread(Request<Thread> request) =>
        await proxyWrappers.Send<Thread?>("ForumRunAddThread", request);

    public async Task<Response> SaveThread(Request<Thread> request) =>
        await proxyWrappers.Send("ForumRunSaveThread", request);

    public async Task<Response> RemoveThread(Request<Thread> request) =>
        await proxyWrappers.Send("ForumRunRemoveThread", request);

    public async Task<Response<IList<Comment>>> FetchComments(Request<Thread> request) =>
        await proxyWrappers.Send<IList<Comment>>("ForumRunFetchComments", request);

    public async Task<Response<Comment?>> AddComment(Request<Comment> request) =>
        await proxyWrappers.Send<Comment?>("ForumRunAddComment", request);

    public async Task<Response> SaveComment(Request<Comment> request) =>
        await proxyWrappers.Send("ForumRunSaveComment", request);

    public async Task<Response> RemoveComment(Request<Comment> request) =>
        await proxyWrappers.Send("ForumRunRemoveComment", request);

    public async Task<Response> SetReaction(Request<CommentReaction> request) =>
        await proxyWrappers.Send("ForumRunSetReaction", request);
}