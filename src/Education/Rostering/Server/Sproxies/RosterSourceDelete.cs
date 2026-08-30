using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterSourceDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, RosterSource value)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterSourceDelete" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", value.Id);
        command.AddParameter("@OrganizationId", value.OrganizationId);
        await command.Execute(connection, transaction);
    }
}