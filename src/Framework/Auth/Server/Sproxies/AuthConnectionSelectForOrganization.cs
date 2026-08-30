using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthConnectionSelectForOrganization
{
    public static async Task<IList<AuthConnection>> Execute(String connection, Guid? organizationId)
    {
        await using var command = Command(organizationId);
        return await command.ReadAll(connection, Read);
    }

    public static async Task<IList<AuthConnection>> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? organizationId)
    {
        await using var command = Command(organizationId);
        return await command.ReadAll(connection, transaction, Read);
    }

    private static SqlCommand Command(Guid? organizationId)
    {
        var command = new SqlCommand { CommandText = "FrameworkAuth.AuthConnectionSelectForOrganization" };
        command.AddParameter("@OrganizationId", organizationId);
        return command;
    }

    private static AuthConnection Read(SqlDataReader reader) => new()
    {
        Id = reader.ReadGuid(0),
        OrganizationId = reader.ReadGuid(1),
        Provider = reader.ReadString(2),
        Tenant = reader.ReadString(3),
        Enabled = reader.ReadBoolean(4) ?? false,
    };
}