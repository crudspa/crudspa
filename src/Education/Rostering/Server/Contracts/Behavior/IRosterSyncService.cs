using Crudspa.Education.Rostering.Shared.Contracts.Data;

namespace Crudspa.Education.Rostering.Server.Contracts.Behavior;

public interface IRosterSyncService
{
    Task<RosterRun> Run(Guid sourceId, String kind, CancellationToken cancellationToken = default);
}