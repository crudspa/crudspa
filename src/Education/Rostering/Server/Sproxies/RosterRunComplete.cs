using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterRunComplete
{
    public static async Task<RosterRun> Execute(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid runId,
        String? checkpoint)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterRunComplete" };
        command.AddParameter("@RosterRunId", runId);
        command.AddParameter("@Checkpoint", 500, checkpoint);
        return (await command.ReadSingle(connection, transaction, RosterRunReader.Read))!;
    }
}