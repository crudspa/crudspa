using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterScheduleDelete
{
    public static async Task Execute(
        SqlConnection connection,
        SqlTransaction? transaction,
        Guid? sessionId,
        Guid? scheduleId)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterScheduleDelete" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ScheduleId", scheduleId);
        await command.Execute(connection, transaction);
    }
}