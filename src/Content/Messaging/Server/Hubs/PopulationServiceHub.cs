using PermissionIds = Crudspa.Content.Messaging.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub
{
    public async Task<Response<PopulationRefreshResult?>> PopulationRefresh(Request<PopulationRefresh> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await PopulationService.Refresh(request));
    }

    public async Task<Response<IList<Population>>> PopulationFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await PopulationService.FetchForPortal(request));
    }

    public async Task<Response<Population?>> PopulationFetch(Request<Population> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await PopulationService.Fetch(request));
    }

    public async Task<Response<IList<PopulationToken>>> PopulationFetchTokens(Request<Population> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await PopulationService.FetchTokens(request));
    }

    public async Task<Response<Population?>> PopulationAdd(Request<Population> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await PopulationService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new PopulationAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> PopulationSave(Request<Population> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await PopulationService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new PopulationSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> PopulationRemove(Request<Population> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await PopulationService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new PopulationRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }
}