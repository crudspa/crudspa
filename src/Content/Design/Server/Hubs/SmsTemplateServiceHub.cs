using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<SmsTemplate>>> SmsTemplateSearchForMembership(Request<SmsTemplateSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsTemplateService.SearchForMembership(request);
        });
    }

    public async Task<Response<IList<SmsTemplate>>> SmsTemplateSearchForPortal(Request<SmsTemplateSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await SmsTemplateService.SearchForPortal(request);
        });
    }

    public async Task<Response<SmsTemplate?>> SmsTemplateFetch(Request<SmsTemplate> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsTemplateService.Fetch(request));
    }

    public async Task<Response<SmsTemplate?>> SmsTemplateAdd(Request<SmsTemplate> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsTemplateService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsTemplateAdded
                {
                    Id = response.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> SmsTemplateSave(Request<SmsTemplate> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsTemplateService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsTemplateSaved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> SmsTemplateRemove(Request<SmsTemplate> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await SmsTemplateService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new SmsTemplateRemoved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }
}