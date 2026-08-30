using Crudspa.Education.Rostering.Server.Contracts.Behavior;
using Crudspa.Education.Rostering.Server.Sproxies;
using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Education.Rostering.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Server.Contracts.Behavior;

namespace Crudspa.Education.Rostering.Server.Services;

public class RosterSyncServiceSql(
    RosterProviderRegistry providers,
    IEnumerable<IRosterPostProcessor> postProcessors,
    ISqlWrappers sqlWrappers,
    ICryptographyService cryptographyService,
    ILogger<RosterSyncServiceSql> logger)
    : IRosterSyncService
{
    public async Task<RosterRun> Run(
        Guid sourceId,
        String kind = RosterRunKinds.Full,
        CancellationToken cancellationToken = default)
    {
        var source = await sqlWrappers.WithConnection(async (connection, transaction) =>
            await RosterSourceSelect.Execute(connection, transaction, sourceId))
            ?? throw new InvalidOperationException($"Roster source '{sourceId}' was not found.");

        return await Run(source, kind, cancellationToken);
    }

    private async Task<RosterRun> Run(
        RosterSource source,
        String kind,
        CancellationToken cancellationToken)
    {
        var sourceId = source.Id!.Value;
        if (!String.IsNullOrWhiteSpace(source.ClientSecret))
            source.ClientSecret = cryptographyService.Unprotect(source.ClientSecret!);

        var provider = providers.Find(source.Provider!)
            ?? throw new InvalidOperationException($"Roster provider '{source.Provider}' is not available.");

        var run = await sqlWrappers.WithTransaction(async (connection, transaction) =>
            await RosterRunStart.Execute(connection, transaction, sourceId, kind));

        var sink = new RosterStageSinkSql(sqlWrappers, run.Id);

        try
        {
            await provider.Stage(new(source, kind), sink, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var completed = await sqlWrappers.WithTransaction(async (connection, transaction) =>
                await RosterRunComplete.Execute(connection, transaction, run.Id, sink.Checkpoint));

            if (source.Mode != RosterModes.Authoritative || completed.Status != RosterRunStatuses.Staged)
                return completed;

            var processors = postProcessors.Where(x => x.AppliesTo(source)).ToList();
            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var result = await RosterRunApply.Execute(connection, transaction, run.Id);
                foreach (var processor in processors)
                    await processor.Apply(connection, transaction, source, result, cancellationToken);
                return result;
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Roster run {runId} failed for source {sourceId}.", run.Id, sourceId);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
                await RosterRunFail.Execute(connection, transaction, run.Id));

            throw;
        }
    }
}