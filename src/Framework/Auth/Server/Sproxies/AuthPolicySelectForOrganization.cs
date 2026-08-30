using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthPolicySelectForOrganization
{
    public static async Task<IList<AuthPolicy>> Execute(String connection, Guid? organizationId)
    {
        await using var command = Command(organizationId);
        return await command.ReadAll(connection, Read);
    }

    public static async Task<IList<AuthPolicy>> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? organizationId)
    {
        await using var command = Command(organizationId);
        return await command.ReadAll(connection, transaction, Read);
    }

    private static SqlCommand Command(Guid? organizationId)
    {
        var command = new SqlCommand { CommandText = "FrameworkAuth.AuthPolicySelectForOrganization" };
        command.AddParameter("@OrganizationId", organizationId);
        return command;
    }

    private static AuthPolicy Read(SqlDataReader reader) => new()
    {
        Id = reader.ReadGuid(0),
        OrganizationId = reader.ReadGuid(1),
        AuthConnectionId = reader.ReadGuid(2),
        Audience = reader.ReadString(3),
        Key = reader.ReadString(4),
        IdleTimeoutMinutes = reader.ReadInt32(5) ?? 0,
        AbsoluteTimeoutMinutes = reader.ReadInt32(6) ?? 0,
        Persist = reader.ReadBoolean(7) ?? false,
        AutoRedirect = reader.ReadBoolean(8) ?? false,
        Fallback = reader.ReadBoolean(9) ?? false,
        Enabled = reader.ReadBoolean(10) ?? false,
    };
}