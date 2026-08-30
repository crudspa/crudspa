using Crudspa.Education.Rostering.Shared.Contracts.Ids;

namespace Crudspa.Education.Rostering.Shared.Contracts.Data;

public class RosterSyncConfig
{
    public Guid? SourceId { get; set; }
    public String Kind { get; set; } = RosterRunKinds.Full;
}