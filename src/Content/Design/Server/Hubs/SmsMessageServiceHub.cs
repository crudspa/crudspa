using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<SmsMessage>>> SmsMessageSearchForPortal(Request<SmsMessageSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsMessageService.SearchForPortal(request);
        });
    }

    public async Task<Response<IList<SmsMessage>>> SmsMessageSearchForMembership(Request<SmsMessageSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsMessageService.SearchForMembership(request);
        });
    }

    public async Task<Response<IList<SmsMessage>>> SmsMessageSearchForContactPhone(Request<SmsMessageSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsMessageService.SearchForContactPhone(request);
        });
    }

    public async Task<Response<IList<SmsMessage>>> SmsMessageSearchForContact(Request<SmsMessageSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsMessageService.SearchForContact(request);
        });
    }

    public async Task<Response<IList<SmsMessage>>> SmsMessageSearchThread(Request<SmsMessage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsMessageService.SearchThread(request));
    }

    public async Task<Response<SmsMessage?>> SmsMessageFetch(Request<SmsMessage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsMessageService.Fetch(request));
    }

    public async Task<Response<SmsMessage?>> SmsMessageReply(Request<SmsMessage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsMessageService.Reply(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsMessageAdded
                {
                    Id = response.Value!.Id,
                    MembershipId = response.Value.MembershipId,
                });

            return response;
        });
    }
}