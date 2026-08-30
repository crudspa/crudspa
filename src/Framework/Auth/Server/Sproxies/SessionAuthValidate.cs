using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class SessionAuthValidate
{
    public static async Task<SessionAuthState?> Execute(String connection, Guid sessionId, Guid portalId, DateTimeOffset lastActivity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkAuth.SessionAuthValidate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@LastActivity", lastActivity);

        return await command.ReadSingle(connection, reader => new SessionAuthState
        {
            AuthPolicyId = reader.ReadGuid(0),
            LastActivity = reader.ReadDateTimeOffset(1),
            IdleTimeoutMinutes = reader.ReadInt32(2) ?? 0,
            IdleExpires = reader.ReadDateTimeOffset(3),
            AbsoluteExpires = reader.ReadDateTimeOffset(4),
        });
    }
}