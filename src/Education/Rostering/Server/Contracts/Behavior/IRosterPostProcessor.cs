using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Contracts.Behavior;

public interface IRosterPostProcessor
{
    Boolean AppliesTo(RosterSource source);
    Task Apply(SqlConnection connection, SqlTransaction transaction, RosterSource source, RosterRun run, CancellationToken cancellationToken);
}