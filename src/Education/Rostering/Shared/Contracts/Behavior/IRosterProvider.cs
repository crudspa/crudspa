using Crudspa.Education.Rostering.Shared.Contracts.Data;

namespace Crudspa.Education.Rostering.Shared.Contracts.Behavior;

public interface IRosterProvider
{
    String Key { get; }
    Task Stage(RosterContext context, IRosterSink sink, CancellationToken cancellationToken = default);
}