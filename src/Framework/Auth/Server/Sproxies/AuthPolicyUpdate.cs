using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthPolicyUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AuthPolicy value)
    {
        await using var command = AuthPolicyInsert.Command(sessionId, value, "FrameworkAuth.AuthPolicyUpdate");
        await command.Execute(connection, transaction);
    }
}