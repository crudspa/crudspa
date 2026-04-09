using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;
using MemberSearch = Crudspa.Content.Design.Shared.Contracts.Data.MemberSearch;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Member>>> MemberSearchForMembership(Request<MemberSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await MemberService.SearchForMembership(request);
        });
    }

    public async Task<Response<Member?>> MemberFetch(Request<Member> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await MemberService.Fetch(request));
    }

    public async Task<Response<Member?>> MemberAdd(Request<Member> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await MemberService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new MemberAdded
                {
                    Id = response.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> MemberSave(Request<Member> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await MemberService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new MemberSaved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }

    public async Task<Response> MemberRemove(Request<Member> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            var response = await MemberService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Memberships, new MemberRemoved
                {
                    Id = request.Value.Id,
                    MembershipId = request.Value.MembershipId,
                });

            return response;
        });
    }
}