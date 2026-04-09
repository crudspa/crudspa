using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.District.Server.Hubs;

public partial class DistrictHub
{
    public async Task<Response<IList<DistrictContact>>> DistrictContactSearch(Request<DistrictContactSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await DistrictContactService.Search(request);
        });
    }

    public async Task<Response<DistrictContact?>> DistrictContactFetch(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await DistrictContactService.Fetch(request));
    }

    public async Task<Response<DistrictContact?>> DistrictContactAdd(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await DistrictContactService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new DistrictContactAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> DistrictContactSave(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await DistrictContactService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new DistrictContactSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> DistrictContactRemove(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
        {
            var response = await DistrictContactService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Contacts, new DistrictContactRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> DistrictContactFetchRoleNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await DistrictContactService.FetchRoleNames(request));
    }

    public async Task<Response> DistrictContactSendAccessCode(Request<DistrictContact> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Contacts, async session =>
            await DistrictContactService.SendAccessCode(request));
    }
}