using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Contracts.Behavior;

public interface IRosterConfigRepository
{
    Task<RosterConfig> Select(String connection, Guid? organizationId);
    Task Save(SqlConnection connection, SqlTransaction transaction, Guid? sessionId, Guid? organizationId, Guid? deviceId, RosterConfig config);
}