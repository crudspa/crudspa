using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<EmailTemplate>>> EmailTemplateSearchForMembership(Request<EmailTemplateSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await EmailTemplateService.SearchForMembership(request);
        });
    }

    public async Task<Response<EmailTemplate?>> EmailTemplateFetch(Request<EmailTemplate> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await EmailTemplateService.Fetch(request));
    }

    public async Task<Response<EmailTemplate?>> EmailTemplateAdd(Request<EmailTemplate> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await EmailTemplateService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new EmailTemplateAdded
                {
                    Id = response.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> EmailTemplateSave(Request<EmailTemplate> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await EmailTemplateService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new EmailTemplateSaved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> EmailTemplateRemove(Request<EmailTemplate> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await EmailTemplateService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new EmailTemplateRemoved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Token>>> EmailTemplateFetchTokens(Request<Membership> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await EmailTemplateService.FetchTokens(request));
    }
}