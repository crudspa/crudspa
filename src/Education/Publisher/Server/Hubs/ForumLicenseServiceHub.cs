using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<ForumLicense>>> ForumLicenseFetchForLicense(Request<License> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await ForumLicenseService.FetchForLicense(request));
    }

    public async Task<Response<ForumLicense?>> ForumLicenseFetch(Request<ForumLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await ForumLicenseService.Fetch(request));
    }

    public async Task<Response<ForumLicense?>> ForumLicenseAdd(Request<ForumLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await ForumLicenseService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new ForumLicenseAdded
                {
                    Id = response.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> ForumLicenseSave(Request<ForumLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await ForumLicenseService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new ForumLicenseSaved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response> ForumLicenseRemove(Request<ForumLicense> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await ForumLicenseService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new ForumLicenseRemoved
                {
                    Id = request.Value.Id,
                    LicenseId = request.Value.LicenseId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ForumLicenseFetchForumNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await ForumLicenseService.FetchForumNames(request));
    }
}