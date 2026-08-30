using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterScheduleSave
{
    public static async Task<Guid?> Execute(
        SqlConnection connection,
        SqlTransaction? transaction,
        Guid? sessionId,
        Guid? deviceId,
        RosterSource source)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterScheduleSave" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DeviceId", deviceId);
        command.AddParameter("@RosterSourceId", source.Id);
        command.AddParameter("@ScheduleHour", source.ScheduleHour);
        command.AddParameter("@ScheduleMinute", source.ScheduleMinute);
        command.AddParameter("@ScheduleTimeZoneId", 32, source.ScheduleTimeZoneId);
        var output = command.AddInputOutputParameter("@ScheduleId", source.ScheduleId);
        await command.Execute(connection, transaction);
        return output.Value is DBNull ? null : (Guid?)output.Value;
    }
}