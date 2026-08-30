using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterSourceUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, RosterSource value)
    {
        await using var command = RosterSourceInsert.Command(sessionId, value, "EducationRostering.RosterSourceUpdate");
        await command.Execute(connection, transaction);
    }
}