using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthPolicyInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AuthPolicy value)
    {
        await using var command = Command(sessionId, value, "FrameworkAuth.AuthPolicyInsert");
        await command.Execute(connection, transaction);
        return value.Id;
    }

    internal static SqlCommand Command(Guid? sessionId, AuthPolicy value, String commandText)
    {
        var command = new SqlCommand { CommandText = commandText };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", value.Id);
        command.AddParameter("@OrganizationId", value.OrganizationId);
        command.AddParameter("@AuthConnectionId", value.AuthConnectionId);
        command.AddParameter("@Audience", 25, value.Audience);
        command.AddParameter("@Key", 75, value.Key);
        command.AddParameter("@IdleTimeoutMinutes", value.IdleTimeoutMinutes);
        command.AddParameter("@AbsoluteTimeoutMinutes", value.AbsoluteTimeoutMinutes);
        command.AddParameter("@Persist", value.Persist);
        command.AddParameter("@AutoRedirect", value.AutoRedirect);
        command.AddParameter("@Fallback", value.Fallback);
        command.AddParameter("@Enabled", value.Enabled);
        return command;
    }
}