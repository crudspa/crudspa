using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterSourceInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, RosterSource value)
    {
        await using var command = Command(sessionId, value, "EducationRostering.RosterSourceInsert");
        await command.Execute(connection, transaction);
    }

    internal static SqlCommand Command(Guid? sessionId, RosterSource value, String commandText)
    {
        var command = new SqlCommand { CommandText = commandText };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", value.Id);
        command.AddParameter("@OrganizationId", value.OrganizationId);
        command.AddParameter("@Provider", 75, value.Provider);
        command.AddParameter("@Tenant", 255, value.Tenant);
        command.AddParameter("@ClientId", 255, value.ClientId);
        command.AddParameter("@ClientSecret", 500, value.ClientSecret);
        command.AddParameter("@TokenUrl", 500, value.TokenUrl);
        command.AddParameter("@BaseUrl", 500, value.BaseUrl);
        command.AddParameter("@Mode", 25, value.Mode);
        command.AddParameter("@ScheduleId", value.ScheduleId);
        command.AddParameter("@Recurring", value.Recurring);
        command.AddParameter("@ScheduleHour", value.ScheduleHour);
        command.AddParameter("@ScheduleMinute", value.ScheduleMinute);
        command.AddParameter("@ScheduleTimeZoneId", 32, value.ScheduleTimeZoneId);
        return command;
    }
}