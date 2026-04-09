using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;
using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;
using PostAdded = Crudspa.Education.School.Shared.Contracts.Events.PostAdded;
using PostRemoved = Crudspa.Education.School.Shared.Contracts.Events.PostRemoved;
using PostSaved = Crudspa.Education.School.Shared.Contracts.Events.PostSaved;
using PostSearch = Crudspa.Education.School.Shared.Contracts.Data.PostSearch;

namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub
{
    public async Task<Response<IList<Post>>> SchoolPostSearchForForum(Request<PostSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SchoolPostService.SearchForForum(request);
        });
    }

    public async Task<Response<IList<Post>>> SchoolPostFetchTreeForParent(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await SchoolPostService.FetchTreeForParent(request));
    }

    public async Task<Response<Post?>> SchoolPostFetch(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await SchoolPostService.Fetch(request));
    }

    public async Task<Response<Post?>> SchoolPostAdd(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await SchoolPostService.Add(request);

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

    public async Task<Response> SchoolPostSave(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await SchoolPostService.Save(request);

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

    public async Task<Response> SchoolPostRemove(Request<Post> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await SchoolPostService.Remove(request);

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

    public async Task<Response<IList<Orderable>>> SchoolPostFetchGradeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await SchoolPostService.FetchGradeNames(request));
    }

    public async Task<Response<IList<Orderable>>> SchoolPostFetchClassroomTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await SchoolPostService.FetchClassroomTypeNames(request));
    }

    public async Task<Response> SchoolPostAddReaction(Request<PostReaction> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await SchoolPostService.AddReaction(request);

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