using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<DistrictLicense>>> DistrictLicenseFetchForLicense(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await DistrictLicenseService.FetchForLicense(request));
    }

    public async Task<Response<DistrictLicense?>> DistrictLicenseFetch(Request<DistrictLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await DistrictLicenseService.Fetch(request));
    }

    public async Task<Response<DistrictLicense?>> DistrictLicenseAdd(Request<DistrictLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await DistrictLicenseService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new DistrictLicenseAdded
                {
                    Id = response.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> DistrictLicenseSave(Request<DistrictLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await DistrictLicenseService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new DistrictLicenseSaved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> DistrictLicenseRemove(Request<DistrictLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await DistrictLicenseService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new DistrictLicenseRemoved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> DistrictLicenseSaveRelations(Request<DistrictLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await DistrictLicenseService.SaveRelations(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new DistrictLicenseRelationsSaved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> DistrictLicenseFetchDistrictNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await DistrictLicenseService.FetchDistrictNames(request));
    }
}