using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthRouteSelect
{
    public static async Task<IList<AuthRoute>> Execute(String connection, String audience, String? key = null)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.AuthRouteSelect" };
        command.AddParameter("@Audience", 25, audience);
        command.AddParameter("@Key", 75, key);
        return await command.ReadAll(connection, Read);
    }

    private static AuthRoute Read(SqlDataReader reader) => new()
    {
        Key = reader.ReadString(0)!,
        Name = reader.ReadString(1)!,
        Provider = reader.ReadString(2)!,
        Tenant = reader.ReadString(3),
        Audience = reader.ReadString(4)!,
    };
}