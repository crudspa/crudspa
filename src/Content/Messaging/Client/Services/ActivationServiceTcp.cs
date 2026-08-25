namespace Crudspa.Content.Messaging.Client.Services;

public class ActivationServiceTcp(IProxyWrappers proxyWrappers) : IActivationService
{
    public async Task<Response<IList<ActivationTarget>>> SearchTargets(Request<ActivationTargetSearch> request) =>
        await proxyWrappers.Send<IList<ActivationTarget>>("ActivationTargetSearch", request);

    public async Task<Response<IList<CampaignScheduleOption>>> FetchScheduleOptions(Request<ActivationTarget> request) =>
        await proxyWrappers.Send<IList<CampaignScheduleOption>>("CampaignScheduleOptionFetch", request);

    public async Task<Response<IList<Activation>>> SearchForOrganization(Request<ActivationSearch> request) =>
        await proxyWrappers.Send<IList<Activation>>("ActivationSearchForOrganization", request);

    public async Task<Response<IList<Activation>>> FetchForCampaign(Request<Campaign> request) =>
        await proxyWrappers.Send<IList<Activation>>("ActivationFetchForCampaign", request);

    public async Task<Response<CampaignActivationResult?>> Activate(Request<CampaignActivation> request) =>
        await proxyWrappers.Send<CampaignActivationResult?>("CampaignActivate", request);

    public async Task<Response<CampaignScheduleConfiguration?>> FetchSchedule(Request<Activation> request) =>
        await proxyWrappers.Send<CampaignScheduleConfiguration?>("CampaignScheduleFetch", request);

    public async Task<Response> SaveSchedule(Request<CampaignScheduleConfiguration> request) =>
        await proxyWrappers.Send("CampaignScheduleSave", request);
}