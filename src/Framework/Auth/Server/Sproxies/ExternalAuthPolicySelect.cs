using Crudspa.Framework.Core.Server.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class ExternalAuthPolicySelect
{
    public static async Task<ExternalAuthRoute?> Execute(String connection, Guid userId)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.ExternalAuthPolicySelect" };
        command.AddParameter("@UserId", userId);

        return await command.ReadSingle<ExternalAuthRoute>(connection, reader => new()
        {
            Provider = reader.ReadString(0)!,
            Tenant = reader.ReadString(1)!,
            Audience = reader.ReadString(2)!,
        });
    }
}