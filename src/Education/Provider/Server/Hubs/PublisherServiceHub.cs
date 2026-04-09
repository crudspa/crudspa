using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Provider.Server.Hubs;

public partial class ProviderHub
{
    public async Task<Response<IList<Publisher>>> PublisherSearch(Request<PublisherSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await PublisherService.Search(request);
        });
    }

    public async Task<Response<Publisher?>> PublisherFetch(Request<Publisher> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
            await PublisherService.Fetch(request));
    }

    public async Task<Response<Publisher?>> PublisherAdd(Request<Publisher> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
        {
            var response = await PublisherService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publishers, new PublisherAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> PublisherSave(Request<Publisher> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
        {
            var response = await PublisherService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publishers, new PublisherSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> PublisherRemove(Request<Publisher> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
        {
            var response = await PublisherService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Publishers, new PublisherRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> PublisherFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Publishers, async session =>
            await PublisherService.FetchPermissionNames(request));
    }
}