namespace Crudspa.Framework.Jobs.Server.Actions;

public class KeepAlive(ILogger<KeepAlive> logger)
    : IJobAction
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(30) };

    public KeepAliveConfig Config { get; set; } = new();

    public void Configure(Guid? sessionId, String json)
    {
        Config = json.FromJson<KeepAliveConfig>() ?? new();
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        var errors = Config.Validate();
        if (errors.HasItems())
        {
            logger.LogError("Keep Alive config is invalid. {errors}", errors.ToStringWithSpaces());
            return false;
        }

        var results = await Task.WhenAll(Config.GetUrls().Select(Fetch));
        return results.All(x => x);
    }

    private async Task<Boolean> Fetch(String url)
    {
        try
        {
            var started = DateTimeOffset.UtcNow;
            using var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var elapsed = DateTimeOffset.UtcNow - started;
            var elapsedMs = (Int32)Math.Ceiling(elapsed.TotalMilliseconds);

            if (response.IsSuccessStatusCode)
            {
                if (elapsedMs <= Config.ExpectedMaxLatencyMs)
                {
                    logger.LogInformation("Keep Alive fetched {url} with status {statusCode} in {elapsedMs} ms.", url, (Int32)response.StatusCode, elapsedMs);
                    return true;
                }

                logger.LogWarning("Keep Alive fetched {url} with status {statusCode} in {elapsedMs} ms, exceeding the {expectedMaxLatencyMs} ms limit.",
                    url,
                    (Int32)response.StatusCode,
                    elapsedMs,
                    Config.ExpectedMaxLatencyMs);

                return false;
            }

            logger.LogError("Keep Alive fetch failed for {url} with status {statusCode} in {elapsedMs} ms.", url, (Int32)response.StatusCode, elapsedMs);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while fetching {url}.", url);
            return false;
        }
    }
}