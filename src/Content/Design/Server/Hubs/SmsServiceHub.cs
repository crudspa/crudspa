using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Sms>>> SmsSearchForMembership(Request<SmsSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsService.SearchForMembership(request);
        });
    }

    public async Task<Response<IList<Sms>>> SmsSearchForPortal(Request<SmsSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsService.SearchForPortal(request);
        });
    }

    public async Task<Response<Sms?>> SmsFetch(Request<Sms> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsService.Fetch(request));
    }

    public async Task<Response<Sms?>> SmsAdd(Request<Sms> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsAdded
                {
                    Id = response.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> SmsSave(Request<Sms> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsSaved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> SmsRemove(Request<Sms> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsRemoved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response<IList<SmsTemplateFull>>> SmsFetchSmsTemplates(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsService.FetchSmsTemplates(request));
    }
}