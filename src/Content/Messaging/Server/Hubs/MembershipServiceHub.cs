using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;
using Membership = Crudspa.Content.Messaging.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub
{
    public async Task<Response<IList<Membership>>> MembershipFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await MembershipService.FetchForPortal(request));
    }

    public async Task<Response<Membership?>> MembershipFetch(Request<Membership> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await MembershipService.Fetch(request));
    }

}