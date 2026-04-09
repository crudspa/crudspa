using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.District.Server.Hubs;

using District = Shared.Contracts.Data.District;

public partial class DistrictHub
{
    public async Task<Response<District?>> DistrictFetch(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await DistrictService.Fetch(request));
    }

    public async Task<Response> DistrictSave(Request<District> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await DistrictService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new DistrictSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> DistrictFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await DistrictService.FetchPermissionNames(request));
    }

    public async Task<Response<IList<Named>>> DistrictFetchCommunityNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await DistrictService.FetchCommunityNames(request));
    }
}