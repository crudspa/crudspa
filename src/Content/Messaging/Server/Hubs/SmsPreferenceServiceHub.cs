using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub
{
    public async Task<Response<IList<SmsPreference>>> SmsPreferenceSearchForPortal(Request<SmsPreferenceSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsPreferenceService.SearchForPortal(request);
        });
    }

    public async Task<Response<SmsPreference?>> SmsPreferenceFetch(Request<SmsPreference> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsPreferenceService.Fetch(request));
    }

    public async Task<Response<SmsPreference?>> SmsPreferenceAdd(Request<SmsPreference> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsPreferenceService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsPreferenceAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> SmsPreferenceSave(Request<SmsPreference> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsPreferenceService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsPreferenceSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> SmsPreferenceRemove(Request<SmsPreference> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsPreferenceService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsPreferenceRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> SmsPreferenceFetchContactNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsPreferenceService.FetchContactNames(request));
    }

    public async Task<Response<IList<Orderable>>> SmsPreferenceFetchContactPhoneNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsPreferenceService.FetchContactPhoneNames(request));
    }
}