using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<CampaignLicense>>> CampaignLicenseFetchForLicense(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await CampaignLicenseService.FetchForLicense(request));
    }

    public async Task<Response<CampaignLicense?>> CampaignLicenseFetch(Request<CampaignLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await CampaignLicenseService.Fetch(request));
    }

    public async Task<Response<CampaignLicense?>> CampaignLicenseAdd(Request<CampaignLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await CampaignLicenseService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new CampaignLicenseAdded
                {
                    Id = response.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> CampaignLicenseSave(Request<CampaignLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await CampaignLicenseService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new CampaignLicenseSaved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> CampaignLicenseRemove(Request<CampaignLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await CampaignLicenseService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new CampaignLicenseRemoved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> CampaignLicenseFetchCampaignNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await CampaignLicenseService.FetchCampaignNames(request));
    }
}