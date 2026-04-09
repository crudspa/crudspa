using PermissionIds = Crudspa.Framework.Core.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

using Publisher = Shared.Contracts.Data.Publisher;

public partial class PublisherHub
{
    public async Task<Response<Publisher?>> PublisherFetch(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await PublisherService.Fetch(request));
    }

    public async Task<Response> PublisherSave(Request<Publisher> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
        {
            var response = await PublisherService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Organization, new PublisherSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Named>>> PublisherFetchPermissionNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Organization, async session =>
            await PublisherService.FetchPermissionNames(request));
    }
}