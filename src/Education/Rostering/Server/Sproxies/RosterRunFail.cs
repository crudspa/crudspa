using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterRunFail
{
    public static async Task Execute(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid runId)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterRunFail" };
        command.AddParameter("@RosterRunId", runId);
        await command.Execute(connection, transaction);
    }
}