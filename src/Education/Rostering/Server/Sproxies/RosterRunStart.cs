using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterRunStart
{
    public static async Task<RosterRun> Execute(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid sourceId,
        String kind)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterRunStart" };
        command.AddParameter("@RosterSourceId", sourceId);
        command.AddParameter("@Kind", 25, kind);
        return (await command.ReadSingle(connection, transaction, RosterRunReader.Read))!;
    }
}