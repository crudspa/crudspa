using PermissionIds = Crudspa.Content.Messaging.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub
{
    public async Task<Response<IList<Stage>>> StageFetchForCampaign(Request<Campaign> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await StageService.FetchForCampaign(request));
    }

    public async Task<Response<Stage?>> StageFetch(Request<Stage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await StageService.Fetch(request));
    }

    public async Task<Response<Stage?>> StageAdd(Request<Stage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await StageService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new StageAdded
                {
                    Id = response.Value.Id,
                    CampaignId = request.Value.CampaignId,
                });

            return response;
        });
    }

    public async Task<Response> StageSave(Request<Stage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await StageService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new StageSaved
                {
                    Id = request.Value.Id,
                    CampaignId = request.Value.CampaignId,
                });

            return response;
        });
    }

    public async Task<Response> StageRemove(Request<Stage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await StageService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new StageRemoved
                {
                    Id = request.Value.Id,
                    CampaignId = request.Value.CampaignId,
                });

            return response;
        });
    }

    public async Task<Response> StageSaveOrder(Request<IList<Stage>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await StageService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Campaigns, new StagesReordered
                {
                    CampaignId = request.Value.First().CampaignId,
                });

            return response;
        });
    }
}