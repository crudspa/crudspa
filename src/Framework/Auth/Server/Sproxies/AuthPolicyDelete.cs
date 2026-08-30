using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthPolicyDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AuthPolicy value)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.AuthPolicyDelete" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", value.Id);
        command.AddParameter("@OrganizationId", value.OrganizationId);
        await command.Execute(connection, transaction);
    }
}