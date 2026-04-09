using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;
using ForumAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.ForumAdded;
using ForumRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.ForumRemoved;
using ForumSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.ForumSaved;
using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Forum>>> PublisherForumSearch(Request<ForumSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await PublisherForumService.Search(request);
        });
    }

    public async Task<Response<Forum?>> PublisherForumFetch(Request<Forum> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await PublisherForumService.Fetch(request));
    }

    public async Task<Response<Forum?>> PublisherForumAdd(Request<Forum> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await PublisherForumService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ForumAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> PublisherForumSave(Request<Forum> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await PublisherForumService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ForumSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> PublisherForumRemove(Request<Forum> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
        {
            var response = await PublisherForumService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Forums, new ForumRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> PublisherForumFetchBodyTemplateNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await PublisherForumService.FetchBodyTemplateNames(request));
    }

    public async Task<Response<IList<Named>>> PublisherForumFetchDistrictNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await PublisherForumService.FetchDistrictNames(request));
    }

    public async Task<Response<IList<Named>>> PublisherForumFetchSchoolNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Forums, async session =>
            await PublisherForumService.FetchSchoolNames(request));
    }
}