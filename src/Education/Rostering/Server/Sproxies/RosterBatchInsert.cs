using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterBatchInsert
{
    public static async Task Execute(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid runId,
        RosterBatch batch)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterBatchInsert" };
        command.AddParameter("@RosterRunId", runId);
        command.AddParameter("@Json", batch.ToJson());
        await command.Execute(connection, transaction);
    }
}