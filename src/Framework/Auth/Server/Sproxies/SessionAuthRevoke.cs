using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class SessionAuthRevoke
{
    public static async Task<Boolean> Execute(String connection, Guid sessionId, Guid portalId, String reason)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkAuth.SessionAuthRevoke";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@Reason", 75, reason);

        return await command.ExecuteQuery(connection, async reader =>
        {
            await reader.ReadAsync();
            return reader.GetBoolean(0);
        });
    }
}