using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Provider.Server.Hubs;

using Provider = Shared.Contracts.Data.Provider;

public partial class ProviderHub
{
    public async Task<Response<Provider?>> ProviderFetch(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await ProviderService.Fetch(request));
    }

    public async Task<Response> ProviderSave(Request<Provider> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await ProviderService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new ProviderSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> ProviderFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await ProviderService.FetchPermissionNames(request));
    }
}