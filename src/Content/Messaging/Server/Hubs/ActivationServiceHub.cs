using PermissionIds = Crudspa.Content.Messaging.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub
{
    public async Task<Response<IList<ActivationTarget>>> ActivationTargetSearch(Request<ActivationTargetSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await ActivationService.SearchTargets(request));
    }

    public async Task<Response<IList<CampaignScheduleOption>>> CampaignScheduleOptionFetch(Request<ActivationTarget> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await ActivationService.FetchScheduleOptions(request));
    }

    public async Task<Response<CampaignActivationResult?>> CampaignActivate(Request<CampaignActivation> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
        {
            var response = await ActivationService.Activate(request);

            if (response.Ok)
            {
                await NotifyOrganization(request.Value.OrganizationId!.Value, PermissionIds.Memberships, new ActivationAdded
                {
                    OrganizationId = request.Value.OrganizationId,
                    CampaignId = request.Value.CampaignId,
                });
            }

            return response;
        });
    }

    public async Task<Response<IList<Activation>>> ActivationSearchForOrganization(Request<ActivationSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await ActivationService.SearchForOrganization(request);
        });
    }

    public async Task<Response<IList<Activation>>> ActivationFetchForCampaign(Request<Campaign> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Campaigns, async session =>
            await ActivationService.FetchForCampaign(request));
    }

    public async Task<Response<CampaignScheduleConfiguration?>> CampaignScheduleFetch(Request<Activation> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await ActivationService.FetchSchedule(request));
    }

    public async Task<Response> CampaignScheduleSave(Request<CampaignScheduleConfiguration> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await ActivationService.SaveSchedule(request));
    }
}