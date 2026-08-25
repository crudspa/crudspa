using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;
using Post = Crudspa.Content.Display.Shared.Contracts.Data.Post;
using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Comment>>> CommentFetchTreeForPost(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Blogs, async session =>
            await CommentService.FetchTreeForPost(request));
    }

    public async Task<Response<IList<Comment>>> CommentFetchTreeForThread(Request<Thread> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await CommentService.FetchTreeForThread(request));
    }

    public async Task<Response<Comment?>> CommentFetch(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var scopeResponse = await ResolveExistingCommentScope(request);
            if (!scopeResponse.Ok)
                return scopeResponse;

            var permissionId = PermissionIdFor(scopeResponse.Value);
            if (!permissionId.HasValue)
                return new("Comment scope is invalid.");

            return await HubWrappers.RequirePermission(request, permissionId.Value, async authorizedSession =>
                scopeResponse);
        });
    }

    public async Task<Response<Comment?>> CommentAdd(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var scopeResponse = await ResolveNewCommentScope(request);
            if (!scopeResponse.Ok)
                return scopeResponse;

            var scope = scopeResponse.Value;
            var permissionId = PermissionIdFor(scope);
            if (!permissionId.HasValue)
                return new("Comment scope is invalid.");

            request.Value.PostId = scope.PostId;
            request.Value.ThreadId = scope.ThreadId;

            return await HubWrappers.RequirePermission(request, permissionId.Value, async authorizedSession =>
            {
                var response = await CommentService.Add(request);

                if (response.Ok)
                {
                    await Notify(request.SessionId, permissionId.Value, new CommentAdded
                    {
                        Id = response.Value.Id,
                        PostId = scope.PostId,
                        ThreadId = scope.ThreadId,
                        ParentId = response.Value.ParentId,
                    });

                    if (scope.ThreadId.HasValue)
                        await GatewayService.Publish(new ForumRunChanged
                        {
                            ThreadId = scope.ThreadId,
                            CommentId = response.Value.Id,
                        });
                }

                return response;
            });
        });
    }

    public async Task<Response> CommentSave(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var scopeResponse = await ResolveExistingCommentScope(request);
            if (!scopeResponse.Ok)
                return ResponseFrom(scopeResponse, "Comment was not found.");

            var scope = scopeResponse.Value;
            var permissionId = PermissionIdFor(scope);
            if (!permissionId.HasValue)
                return new("Comment scope is invalid.");

            request.Value.PostId = scope.PostId;
            request.Value.ThreadId = scope.ThreadId;
            request.Value.ParentId = scope.ParentId;

            return await HubWrappers.RequirePermission(request, permissionId.Value, async authorizedSession =>
            {
                var response = await CommentService.Save(request);

                if (response.Ok)
                {
                    await Notify(request.SessionId, permissionId.Value, new CommentSaved
                    {
                        Id = scope.Id,
                        PostId = scope.PostId,
                        ThreadId = scope.ThreadId,
                        ParentId = scope.ParentId,
                    });

                    if (scope.ThreadId.HasValue)
                        await GatewayService.Publish(new ForumRunChanged
                        {
                            ThreadId = scope.ThreadId,
                            CommentId = scope.Id,
                        });
                }

                return response;
            });
        });
    }

    public async Task<Response> CommentRemove(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var scopeResponse = await ResolveExistingCommentScope(request);
            if (!scopeResponse.Ok)
                return ResponseFrom(scopeResponse, "Comment was not found.");

            var scope = scopeResponse.Value;
            var permissionId = PermissionIdFor(scope);
            if (!permissionId.HasValue)
                return new("Comment scope is invalid.");

            request.Value.PostId = scope.PostId;
            request.Value.ThreadId = scope.ThreadId;
            request.Value.ParentId = scope.ParentId;

            return await HubWrappers.RequirePermission(request, permissionId.Value, async authorizedSession =>
            {
                var response = await CommentService.Remove(request);

                if (response.Ok)
                {
                    await Notify(request.SessionId, permissionId.Value, new CommentRemoved
                    {
                        Id = scope.Id,
                        PostId = scope.PostId,
                        ThreadId = scope.ThreadId,
                        ParentId = scope.ParentId,
                    });

                    if (scope.ThreadId.HasValue)
                        await GatewayService.Publish(new ForumRunChanged
                        {
                            ThreadId = scope.ThreadId,
                            CommentId = scope.Id,
                        });
                }

                return response;
            });
        });
    }

    private async Task<Response<Comment?>> ResolveExistingCommentScope(Request<Comment> request)
    {
        var response = await CommentService.Fetch(new(request.SessionId, new() { Id = request.Value.Id }));

        if (!response.Ok && response.Errors.Count == 0)
            response.AddError("Comment was not found.");

        return response;
    }

    private async Task<Response<Comment?>> ResolveNewCommentScope(Request<Comment> request)
    {
        var comment = request.Value;

        if (comment.ParentId.HasValue)
        {
            var parentResponse = await CommentService.Fetch(new(request.SessionId, new() { Id = comment.ParentId }));

            if (!parentResponse.Ok)
            {
                if (parentResponse.Errors.Count == 0)
                    parentResponse.AddError("Parent comment was not found.");

                return parentResponse;
            }

            return new(new Comment
            {
                PostId = parentResponse.Value.PostId,
                ThreadId = parentResponse.Value.ThreadId,
                ParentId = parentResponse.Value.Id,
            });
        }

        if (comment.PostId.HasValue == comment.ThreadId.HasValue)
            return new("A comment must target exactly one post or thread.");

        if (comment.PostId.HasValue)
        {
            var postResponse = await PostService.Fetch(new(request.SessionId, new() { Id = comment.PostId }));
            if (!postResponse.Ok)
            {
                var response = new Response<Comment?>("Comment post was not found.");
                response.AddErrors(postResponse.Errors);
                return response;
            }

            return new(new Comment { PostId = postResponse.Value.Id });
        }

        var threadResponse = await ThreadService.Fetch(new(request.SessionId, new() { Id = comment.ThreadId }));
        if (!threadResponse.Ok)
        {
            var response = new Response<Comment?>("Comment thread was not found.");
            response.AddErrors(threadResponse.Errors);
            return response;
        }

        return new(new Comment { ThreadId = threadResponse.Value.Id });
    }

    private static Guid? PermissionIdFor(Comment comment)
    {
        if (comment.PostId.HasValue && !comment.ThreadId.HasValue)
            return PermissionIds.Blogs;

        if (comment.ThreadId.HasValue && !comment.PostId.HasValue)
            return PermissionIds.Forums;

        return null;
    }

    private static Response ResponseFrom(Response<Comment?> response, String fallbackMessage)
    {
        var ret = new Response();
        ret.AddErrors(response.Errors);

        if (ret.Errors.Count == 0)
            ret.AddError(fallbackMessage);

        return ret;
    }
}