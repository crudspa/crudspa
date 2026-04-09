using Crudspa.Content.Display.Shared.Contracts.Events;

namespace Crudspa.Framework.Jobs.Server.Actions;

public class SanitizeHtml(
    ILogger<SanitizeHtml> logger,
    IFrameworkActionService frameworkActionService,
    IGatewayService gatewayService)
    : IJobAction
{
    public SanitizeHtmlConfig Config { get; set; } = new();
    private Guid? _sessionId;

    public void Configure(Guid? sessionId, String json)
    {
        Config = json.FromJson<SanitizeHtmlConfig>() ?? new();
        _sessionId = sessionId;
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            var validation = Config.Validate();

            if (validation.HasItems())
            {
                logger.LogError("Sanitize HTML config is invalid. {errors}", validation.Select(x => x.Message));
                return false;
            }

            var response = await frameworkActionService.SanitizeHtml(new(_sessionId), Config);

            if (!response.Ok || response.Value is null)
            {
                logger.LogError("Call to IFrameworkActionService.SanitizeHtml() failed. {errors}", response.ErrorMessages);
                return false;
            }

            var result = response.Value;

            logger.LogInformation(
                "Sanitize HTML completed. Scanned {rowsScanned} rows, updated {rowsUpdated}, skipped {rowsSkippedRequired} required-empty rows across {targets}.",
                result.RowsScanned,
                result.RowsUpdated,
                result.RowsSkippedRequired,
                result.Targets.Count);

            foreach (var target in result.Targets.Where(x => x.RowsScanned > 0 || x.RowsUpdated > 0 || x.RowsSkippedRequired > 0))
            {
                logger.LogInformation(
                    "Sanitize HTML target {target}: scanned {rowsScanned}, updated {rowsUpdated}, skipped {rowsSkippedRequired}.",
                    target.Name,
                    target.RowsScanned,
                    target.RowsUpdated,
                    target.RowsSkippedRequired);
            }

            foreach (var pageId in result.AffectedPageIds.Distinct())
                await gatewayService.Publish(new PageContentChanged { Id = pageId });

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sanitizing HTML.");
            return false;
        }
    }
}