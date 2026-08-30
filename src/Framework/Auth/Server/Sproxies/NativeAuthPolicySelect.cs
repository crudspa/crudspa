using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class NativeAuthPolicySelect
{
    public static async Task<String?> Execute(String connection, Guid userId)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.NativeAuthPolicySelect" };
        command.AddParameter("@UserId", userId);
        return await command.ReadSingle(connection, reader => reader.ReadString(0));
    }
}