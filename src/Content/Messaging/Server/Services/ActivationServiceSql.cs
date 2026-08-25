namespace Crudspa.Content.Messaging.Server.Services;

public class ActivationServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    IEnumerable<IActivationTargetProvider> targetProviders,
    IPopulationService populationService)
    : IActivationService
{
    private readonly ActivationTargetProviderRegistry _targetProviders = new(targetProviders);
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ActivationTarget>>> SearchTargets(Request<ActivationTargetSearch> request)
    {
        return await wrappers.Validate<IList<ActivationTarget>, ActivationTargetSearch>(request, async response =>
        {
            var provider = _targetProviders.Fetch(request.Value.PortalId!.Value);
            return await provider.Search(request.SessionId, request.Value.CampaignId, request.Value.Text);
        });
    }

    public async Task<Response<IList<CampaignScheduleOption>>> FetchScheduleOptions(Request<ActivationTarget> request)
    {
        return await wrappers.Try<IList<CampaignScheduleOption>>(request, async response =>
            await CampaignScheduleOptionSelect.Execute(Connection, request.SessionId, request.Value.OrganizationId));
    }

    public async Task<Response<CampaignScheduleConfiguration?>> FetchSchedule(Request<Activation> request)
    {
        return await wrappers.Try<CampaignScheduleConfiguration?>(request, async response =>
            await CampaignScheduleSelect.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response> SaveSchedule(Request<CampaignScheduleConfiguration> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var config = configService.Fetch();
            await CampaignScheduleSave.Execute(
                Connection, request.SessionId, request.Value, config.EmailFromName, config.EmailFromAddress);

            var refreshes = await PopulationRefreshForActivationSelect.Execute(
                Connection, request.SessionId, request.Value.ActivationId);
            foreach (var populationRefresh in refreshes)
            {
                var refresh = await populationService.Refresh(new(populationRefresh) { SessionId = request.SessionId });
                if (!refresh.Ok) response.AddErrors(refresh.Errors);
            }
        });
    }

    public async Task<Response<IList<Activation>>> SearchForOrganization(Request<ActivationSearch> request)
    {
        return await wrappers.Try<IList<Activation>>(request, async response =>
        {
            var activations = await ActivationSelectWhereForOrganization.Execute(Connection, request.SessionId, request.Value);

            return activations;
        });
    }

    public async Task<Response<IList<Activation>>> FetchForCampaign(Request<Campaign> request)
    {
        return await wrappers.Try<IList<Activation>>(request, async response =>
        {
            var activations = await ActivationSelectForCampaign.Execute(Connection, request.SessionId, request.Value.Id);

            return activations;
        });
    }

    public async Task<Response<CampaignActivationResult?>> Activate(Request<CampaignActivation> request)
    {
        return await wrappers.Validate<CampaignActivationResult?, CampaignActivation>(request, async response =>
        {
            var campaign = await CampaignSelect.Execute(Connection, request.SessionId, new() { Id = request.Value.CampaignId });

            if (campaign?.PortalId is null)
            {
                response.AddError("The selected campaign could not be found.", nameof(CampaignActivation.CampaignId));
                return null;
            }

            var provider = _targetProviders.Fetch(campaign.PortalId.Value);

            var targetValid = await provider.Validate(request.SessionId, campaign.Id, request.Value.OrganizationId!.Value);
            if (!targetValid)
                response.AddError("The selected district is not available for this campaign.", nameof(CampaignActivation.OrganizationId));

            if (response.Errors.Count > 0)
                return null;

            var stages = await StageSelectForCampaign.Execute(
                Connection, request.SessionId, request.Value.CampaignId);
            var config = configService.Fetch();
            var result = await CampaignActivate.Execute(
                Connection,
                request.SessionId,
                request.Value,
                stages,
                config.EmailFromName,
                config.EmailFromAddress);

            if (result is null)
                return null;

            var refreshes = await PopulationRefreshForBatchSelect.Execute(
                Connection, request.SessionId, request.Value.BatchId);
            foreach (var populationRefresh in refreshes)
            {
                var refresh = await populationService.Refresh(new(populationRefresh)
                {
                    SessionId = request.SessionId,
                });
                if (!refresh.Ok)
                    response.AddErrors(refresh.Errors);
            }

            return result;
        });
    }
}