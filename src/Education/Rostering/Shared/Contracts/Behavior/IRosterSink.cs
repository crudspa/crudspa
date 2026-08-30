using Crudspa.Education.Rostering.Shared.Contracts.Data;

namespace Crudspa.Education.Rostering.Shared.Contracts.Behavior;

public interface IRosterSink
{
    Task Write(RosterBatch batch, CancellationToken cancellationToken = default);
}