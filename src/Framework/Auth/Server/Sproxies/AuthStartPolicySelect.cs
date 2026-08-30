using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthStartPolicySelect
{
    public static async Task<IList<AuthStartPolicy>> Execute(
        String connection,
        String? provider,
        String audience,
        String tenant)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.AuthStartPolicySelect" };
        command.AddParameter("@Provider", 75, provider);
        command.AddParameter("@Audience", 25, audience);
        command.AddParameter("@Tenant", 255, tenant);

        return await command.ReadAll<AuthStartPolicy>(connection, reader => new()
        {
            Provider = reader.ReadString(0),
            Tenant = reader.ReadString(1),
            AutoRedirect = reader.ReadBoolean(2) ?? false,
            Fallback = reader.ReadBoolean(3) ?? false,
        });
    }
}