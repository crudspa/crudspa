
using Post = Crudspa.Content.Display.Shared.Contracts.Data.Post;

using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Design.Client.Services;

public class CommentServiceTcp(IProxyWrappers proxyWrappers) : ICommentService
{
    public async Task<Response<IList<Comment>>> FetchTreeForPost(Request<Post> request) =>
        await proxyWrappers.Send<IList<Comment>>("CommentFetchTreeForPost", request);

    public async Task<Response<IList<Comment>>> FetchTreeForThread(Request<Thread> request) =>
        await proxyWrappers.Send<IList<Comment>>("CommentFetchTreeForThread", request);

    public async Task<Response<Comment?>> Fetch(Request<Comment> request) =>
        await proxyWrappers.Send<Comment?>("CommentFetch", request);

    public async Task<Response<Comment?>> Add(Request<Comment> request) =>
        await proxyWrappers.Send<Comment?>("CommentAdd", request);

    public async Task<Response> Save(Request<Comment> request) =>
        await proxyWrappers.Send("CommentSave", request);

    public async Task<Response> Remove(Request<Comment> request) =>
        await proxyWrappers.Send("CommentRemove", request);
}