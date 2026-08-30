using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthConnectionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AuthConnection value)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.AuthConnectionInsert" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", value.Id);
        command.AddParameter("@OrganizationId", value.OrganizationId);
        command.AddParameter("@Provider", 75, value.Provider);
        command.AddParameter("@Tenant", 255, value.Tenant);
        command.AddParameter("@Enabled", value.Enabled);
        await command.Execute(connection, transaction);
        return value.Id;
    }
}