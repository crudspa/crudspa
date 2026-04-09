using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;
using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
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

    public async Task<Response<Membership?>> MembershipAdd(Request<Membership> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await MembershipService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new MembershipAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> MembershipSave(Request<Membership> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await MembershipService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new MembershipSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> MembershipRemove(Request<Membership> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await MembershipService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new MembershipRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }
}