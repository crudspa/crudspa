using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<PublisherContact>>> PublisherContactSearch(Request<PublisherContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await PublisherContactService.Search(request);
        });
    }

    public async Task<Response<PublisherContact?>> PublisherContactFetch(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await PublisherContactService.Fetch(request));
    }

    public async Task<Response<PublisherContact?>> PublisherContactAdd(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await PublisherContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new PublisherContactAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> PublisherContactSave(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await PublisherContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new PublisherContactSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> PublisherContactRemove(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await PublisherContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new PublisherContactRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> PublisherContactFetchRoleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await PublisherContactService.FetchRoleNames(request));
    }

    public async Task<Response> PublisherContactSendAccessCode(Request<PublisherContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await PublisherContactService.SendAccessCode(request));
    }
}