using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<License>>> LicenseSearch(Request<LicenseSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await LicenseService.Search(request);
        });
    }

    public async Task<Response<License?>> LicenseFetch(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await LicenseService.Fetch(request));
    }

    public async Task<Response<License?>> LicenseAdd(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await LicenseService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new LicenseAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> LicenseSave(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await LicenseService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new LicenseSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> LicenseRemove(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await LicenseService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new LicenseRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }
}