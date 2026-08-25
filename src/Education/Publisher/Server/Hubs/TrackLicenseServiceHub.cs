using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<TrackLicense>>> TrackLicenseFetchForLicense(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await TrackLicenseService.FetchForLicense(request));
    }

    public async Task<Response<TrackLicense?>> TrackLicenseFetch(Request<TrackLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await TrackLicenseService.Fetch(request));
    }

    public async Task<Response<TrackLicense?>> TrackLicenseAdd(Request<TrackLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await TrackLicenseService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new TrackLicenseAdded
                {
                    Id = response.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> TrackLicenseSave(Request<TrackLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await TrackLicenseService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new TrackLicenseSaved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> TrackLicenseRemove(Request<TrackLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await TrackLicenseService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new TrackLicenseRemoved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> TrackLicenseFetchTrackNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await TrackLicenseService.FetchTrackNames(request));
    }
}