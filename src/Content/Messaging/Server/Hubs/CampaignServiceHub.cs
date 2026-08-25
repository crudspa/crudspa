using PermissionIds = Crudspa.Content.Messaging.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub
{
    public async Task<Response<IList<Campaign>>> CampaignFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await CampaignService.FetchForPortal(request));
    }

    public async Task<Response<Campaign?>> CampaignFetch(Request<Campaign> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await CampaignService.Fetch(request));
    }

    public async Task<Response<IList<Named>>> CampaignFetchLicenseNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await CampaignService.FetchLicenseNames(request));
    }

    public async Task<Response<Campaign?>> CampaignAdd(Request<Campaign> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await CampaignService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new CampaignAdded
                {
                    Id = response.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> CampaignSave(Request<Campaign> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await CampaignService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new CampaignSaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> CampaignRemove(Request<Campaign> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await CampaignService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new CampaignRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }
}