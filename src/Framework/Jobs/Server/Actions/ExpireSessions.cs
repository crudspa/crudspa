namespace Crudspa.Framework.Jobs.Server.Actions;

public class ExpireSessions(ILogger<ExpireSessions> logger, IFrameworkActionService frameworkActionService)
    : IJobAction
{
    public ExpireSessionsConfig Config { get; set; } = new();
    private Guid? _sessionId;

    public void Configure(Guid? sessionId, String json)
    {
        Config = json.FromJson<ExpireSessionsConfig>() ?? new();
        _sessionId = sessionId;
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            var response = await frameworkActionService.ExpireSessions(new(_sessionId), Config.SessionLengthInDays);

            if (response.Ok)
                return true;

            logger.LogError("Call to IFrameworkActionService.ExpireSessions() failed. {errors}", response.ErrorMessages);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while expiring sessions.");
            return false;
        }
    }
}