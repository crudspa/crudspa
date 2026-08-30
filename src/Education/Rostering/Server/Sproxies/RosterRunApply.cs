using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterRunApply
{
    public static async Task<RosterRun> Execute(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid runId)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterRunApply" };
        command.AddParameter("@RosterRunId", runId);
        return (await command.ReadSingle(connection, transaction, RosterRunReader.Read, 3600))!;
    }
}