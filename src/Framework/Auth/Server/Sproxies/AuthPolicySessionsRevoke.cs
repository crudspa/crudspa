using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthPolicySessionsRevoke
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, IEnumerable<Guid?> policyIds, String reason)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.AuthPolicySessionsRevoke" };
        command.AddParameter("@PolicyIds", policyIds);
        command.AddParameter("@Reason", 75, reason);
        await command.Execute(connection, transaction);
    }
}