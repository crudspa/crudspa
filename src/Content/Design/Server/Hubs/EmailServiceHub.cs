using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;
using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Email>>> EmailSearchForMembership(Request<EmailSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await EmailService.SearchForMembership(request);
        });
    }

    public async Task<Response<Email?>> EmailFetch(Request<Email> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await EmailService.Fetch(request));
    }

    public async Task<Response<Email?>> EmailAdd(Request<Email> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await EmailService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new EmailAdded
                {
                    Id = response.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> EmailSave(Request<Email> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await EmailService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new EmailSaved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> EmailRemove(Request<Email> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await EmailService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new EmailRemoved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response<IList<EmailTemplateFull>>> EmailFetchEmailTemplates(Request<Membership> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await EmailService.FetchEmailTemplates(request));
    }

    public async Task<Response<IList<Token>>> EmailFetchTokens(Request<Membership> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await EmailService.FetchTokens(request));
    }
}