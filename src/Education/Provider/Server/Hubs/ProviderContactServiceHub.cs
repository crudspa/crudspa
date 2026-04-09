using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Provider.Server.Hubs;

public partial class ProviderHub
{
    public async Task<Response<IList<ProviderContact>>> ProviderContactSearch(Request<ProviderContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await ProviderContactService.Search(request);
        });
    }

    public async Task<Response<ProviderContact?>> ProviderContactFetch(Request<ProviderContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await ProviderContactService.Fetch(request));
    }

    public async Task<Response<ProviderContact?>> ProviderContactAdd(Request<ProviderContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await ProviderContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new ProviderContactAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ProviderContactSave(Request<ProviderContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await ProviderContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new ProviderContactSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> ProviderContactRemove(Request<ProviderContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await ProviderContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new ProviderContactRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> ProviderContactFetchRoleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await ProviderContactService.FetchRoleNames(request));
    }

    public async Task<Response> ProviderContactSendAccessCode(Request<ProviderContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await ProviderContactService.SendAccessCode(request));
    }
}