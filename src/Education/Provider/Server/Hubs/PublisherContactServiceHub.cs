using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Provider.Server.Hubs;

public partial class ProviderHub
{
    public async Task<Response<IList<PublisherContact>>> PublisherContactSearchForPublisher(Request<PublisherContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await PublisherContactService.SearchForPublisher(request);
        });
    }

    public async Task<Response<PublisherContact?>> PublisherContactFetch(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
            await PublisherContactService.Fetch(request));
    }

    public async Task<Response<PublisherContact?>> PublisherContactAdd(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
        {
            var response = await PublisherContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publishers, new PublisherContactAdded
                {
                    Id = response.Value.Id,
                    PublisherId = request.Value.PublisherId,
                });

            return response;
        });
    }

    public async Task<Response> PublisherContactSave(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
        {
            var response = await PublisherContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publishers, new PublisherContactSaved
                {
                    Id = request.Value.Id,
                    PublisherId = request.Value.PublisherId,
                });

            return response;
        });
    }

    public async Task<Response> PublisherContactRemove(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
        {
            var response = await PublisherContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publishers, new PublisherContactRemoved
                {
                    Id = request.Value.Id,
                    PublisherId = request.Value.PublisherId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> PublisherContactFetchRoleNames(Request<Publisher> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
            await PublisherContactService.FetchRoleNames(request));
    }

    public async Task<Response> PublisherContactSendAccessCode(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
            await PublisherContactService.SendAccessCode(request));
    }
}