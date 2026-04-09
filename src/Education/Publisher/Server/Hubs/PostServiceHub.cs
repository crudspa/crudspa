using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;
using Post = Crudspa.Education.Publisher.Shared.Contracts.Data.Post;
using PostAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.PostAdded;
using PostRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.PostRemoved;
using PostSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.PostSaved;
using PostSearch = Crudspa.Education.Publisher.Shared.Contracts.Data.PostSearch;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Post>>> PublisherPostSearchForForum(Request<PostSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await PublisherPostService.SearchForForum(request);
        });
    }

    public async Task<Response<IList<Post>>> PublisherPostFetchTreeForParent(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await PublisherPostService.FetchTreeForParent(request));
    }

    public async Task<Response<Post?>> PublisherPostFetch(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await PublisherPostService.Fetch(request));
    }

    public async Task<Response<Post?>> PublisherPostAdd(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await PublisherPostService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new PostAdded
                {
                    Id = response.Value.Id,
                    ForumId = request.Value.ForumId,
                    ParentId = request.Value.ParentId,
                });

            return response;
        });
    }

    public async Task<Response> PublisherPostSave(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await PublisherPostService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new PostSaved
                {
                    Id = request.Value.Id,
                    ForumId = request.Value.ForumId,
                    ParentId = request.Value.ParentId,
                });

            return response;
        });
    }

    public async Task<Response> PublisherostRemove(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await PublisherPostService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new PostRemoved
                {
                    Id = request.Value.Id,
                    ForumId = request.Value.ForumId,
                    ParentId = request.Value.ParentId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> PublisherPostFetchGradeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await PublisherPostService.FetchGradeNames(request));
    }

    public async Task<Response<IList<Orderable>>> PublisherPostFetchClassroomTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await PublisherPostService.FetchClassroomTypeNames(request));
    }

    public async Task<Response> PublisherPostAddReaction(Request<PostReaction> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await PublisherPostService.AddReaction(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new PostReactionAdded
                {
                    PostId = request.Value.PostId,
                    ContactId = session.User!.Contact.Id,
                    Character = request.Value.Character,
                });

            return response;
        });
    }
}