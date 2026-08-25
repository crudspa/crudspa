using Crudspa.Content.Messaging.Shared.Contracts.Config;

namespace Crudspa.Content.Jobs.Server.Actions;

public class RefreshPopulations(
    ILogger<RefreshPopulations> logger,
    IPopulationService populationService) : IJobAction
{
    private Guid? _sessionId;
    private PopulationRefreshJobConfig? _config;

    public void Configure(Guid? sessionId, String json)
    {
        _sessionId = sessionId;
        _config = json.FromJson<PopulationRefreshJobConfig>();
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        if (_config is null || _config.Validate().HasItems())
        {
            logger.LogError("Refresh Populations job configuration is invalid.");
            return false;
        }

        foreach (var organizationId in _config.OrganizationIds.Distinct())
        {
            var response = await populationService.Refresh(new(new()
            {
                PopulationId = _config.PopulationId,
                OrganizationId = organizationId,
            }) { SessionId = _sessionId });

            if (!response.Ok)
            {
                logger.LogError(
                    "Population {PopulationId} refresh failed for Organization {OrganizationId}: {Errors}",
                    _config.PopulationId,
                    organizationId,
                    response.ErrorMessages);
                return false;
            }

            logger.LogInformation(
                "Population {PopulationId} refreshed for Organization {OrganizationId}: added {Added}, removed {Removed}, preserved {Preserved}, opted out {OptedOut}.",
                _config.PopulationId,
                organizationId,
                response.Value?.Added,
                response.Value?.Removed,
                response.Value?.Preserved,
                response.Value?.OptedOut);
        }

        return true;
    }
}