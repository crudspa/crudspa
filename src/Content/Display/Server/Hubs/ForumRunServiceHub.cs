using Crudspa.Content.Display.Server;
using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<IList<Forum>>> ForumRunFetchForums(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.FetchForums(request);
            ForumRunResponseRedactor.Redact(response.Value);
            return response;
        });
    }

    public async Task<Response<Forum?>> ForumRunFetchForum(Request<Forum> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.FetchForum(request);
            ForumRunResponseRedactor.Redact(response.Value);
            return response;
        });
    }

    public async Task<Response<IList<Thread>>> ForumRunSearchThreads(Request<ThreadSearch> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            var response = await ForumRunService.SearchThreads(request);
            ForumRunResponseRedactor.Redact(response.Value);
            return response;
        });
    }

    public async Task<Response<Thread?>> ForumRunFetchThread(Request<Thread> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.FetchThread(request);
            ForumRunResponseRedactor.Redact(response.Value);
            return response;
        });
    }

    public async Task<Response<Thread?>> ForumRunAddThread(Request<Thread> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.AddThread(request);
            ForumRunResponseRedactor.Redact(response.Value);

            if (response.Ok)
                await GatewayService.Publish(new ForumRunChanged
                {
                    ForumId = request.Value.ForumId,
                    ThreadId = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ForumRunSaveThread(Request<Thread> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.SaveThread(request);

            if (response.Ok)
                await GatewayService.Publish(new ForumRunChanged
                {
                    ForumId = request.Value.ForumId,
                    ThreadId = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ForumRunRemoveThread(Request<Thread> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.RemoveThread(request);

            if (response.Ok)
                await GatewayService.Publish(new ForumRunChanged
                {
                    ForumId = request.Value.ForumId,
                    ThreadId = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Comment>>> ForumRunFetchComments(Request<Thread> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.FetchComments(request);
            ForumRunResponseRedactor.Redact(response.Value);
            return response;
        });
    }

    public async Task<Response<Comment?>> ForumRunAddComment(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.AddComment(request);
            ForumRunResponseRedactor.Redact(response.Value);

            if (response.Ok)
                await GatewayService.Publish(new ForumRunChanged
                {
                    ThreadId = request.Value.ThreadId,
                    CommentId = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ForumRunSaveComment(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.SaveComment(request);

            if (response.Ok)
                await GatewayService.Publish(new ForumRunChanged
                {
                    ThreadId = request.Value.ThreadId,
                    CommentId = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ForumRunRemoveComment(Request<Comment> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.RemoveComment(request);

            if (response.Ok)
                await GatewayService.Publish(new ForumRunChanged
                {
                    ThreadId = request.Value.ThreadId,
                    CommentId = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ForumRunSetReaction(Request<CommentReaction> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var response = await ForumRunService.SetReaction(request);

            if (response.Ok)
                await GatewayService.Publish(new ForumRunChanged { CommentId = request.Value.CommentId });

            return response;
        });
    }
}