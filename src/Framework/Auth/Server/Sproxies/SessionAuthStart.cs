using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class SessionAuthStart
{
    public static async Task<Boolean> Execute(String connection, Guid sessionId, Guid userId, String provider)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.SessionAuthStart" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@UserId", userId);
        command.AddParameter("@Provider", 75, provider);
        return await command.ExecuteScalarInt(connection) == 1;
    }
}