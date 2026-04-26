

using Post = Crudspa.Content.Display.Shared.Contracts.Data.Post;

using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Comment>>> CommentFetchTreeForPost(Request<Post> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await CommentService.FetchTreeForPost(request));
    }

    public async Task<Response<IList<Comment>>> CommentFetchTreeForThread(Request<Thread> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await CommentService.FetchTreeForThread(request));
    }

    public async Task<Response<Comment?>> CommentFetch(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await CommentService.Fetch(request));
    }

    public async Task<Response<Comment?>> CommentAdd(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await CommentService.Add(request);

            return response;
        });
    }

    public async Task<Response> CommentSave(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await CommentService.Save(request);

            return response;
        });
    }

    public async Task<Response> CommentRemove(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await CommentService.Remove(request);

            return response;
        });
    }
}