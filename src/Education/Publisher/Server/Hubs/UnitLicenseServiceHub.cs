using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<UnitLicense>>> UnitLicenseFetchForLicense(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await UnitLicenseService.FetchForLicense(request));
    }

    public async Task<Response<UnitLicense?>> UnitLicenseFetch(Request<UnitLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await UnitLicenseService.Fetch(request));
    }

    public async Task<Response<UnitLicense?>> UnitLicenseAdd(Request<UnitLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await UnitLicenseService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new UnitLicenseAdded
                {
                    Id = response.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> UnitLicenseSave(Request<UnitLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await UnitLicenseService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new UnitLicenseSaved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> UnitLicenseRemove(Request<UnitLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await UnitLicenseService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new UnitLicenseRemoved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> UnitLicenseSaveRelations(Request<UnitLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await UnitLicenseService.SaveRelations(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new UnitLicenseRelationsSaved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> UnitLicenseFetchUnitNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await UnitLicenseService.FetchUnitNames(request));
    }
}