using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterSourceSelectForOrganization
{
    public static async Task<IList<RosterSource>> Execute(String connection, Guid? organizationId)
    {
        await using var command = Command(organizationId);
        return await command.ReadAll(connection, Read);
    }

    public static async Task<IList<RosterSource>> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? organizationId)
    {
        await using var command = Command(organizationId);
        return await command.ReadAll(connection, transaction, Read);
    }

    private static SqlCommand Command(Guid? organizationId)
    {
        var command = new SqlCommand { CommandText = "EducationRostering.RosterSourceSelectForOrganization" };
        command.AddParameter("@OrganizationId", organizationId);
        return command;
    }

    internal static RosterSource Read(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
            Provider = reader.ReadString(2),
            Tenant = reader.ReadString(3),
            ClientId = reader.ReadString(4),
            ClientSecret = reader.ReadString(5),
            TokenUrl = reader.ReadString(6),
            BaseUrl = reader.ReadString(7),
            Mode = reader.ReadString(8),
            ScheduleId = reader.ReadGuid(9),
            Checkpoint = reader.ReadString(10),
            LastSucceeded = reader.ReadDateTimeOffset(11),
            Recurring = reader.GetBoolean(12),
            ScheduleHour = reader.ReadInt32(13),
            ScheduleMinute = reader.ReadInt32(14),
            ScheduleTimeZoneId = reader.ReadString(15),
        };
    }
}