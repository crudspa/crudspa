using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterJobInsert
{
    public static async Task<Guid?> Execute(
        SqlConnection connection,
        SqlTransaction? transaction,
        Guid? sessionId,
        Guid? deviceId,
        Guid? rosterSourceId)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterJobInsert" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DeviceId", deviceId);
        command.AddParameter("@RosterSourceId", rosterSourceId);
        var output = command.AddOutputParameter("@JobId");
        await command.Execute(connection, transaction);
        return output.Value is DBNull ? null : (Guid?)output.Value;
    }
}