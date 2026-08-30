using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthHandoffRedeem
{
    public static async Task<AuthHandoffRedemption?> Execute(
        String connection,
        Byte[] codeHash,
        Guid portalId,
        Guid sessionId,
        Guid? previousSessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkAuth.AuthHandoffRedeem";

        command.AddParameter("@CodeHash", codeHash);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PreviousSessionId", previousSessionId);

        return await command.ExecuteQuery<AuthHandoffRedemption?>(connection, async reader =>
        {
            if (!await reader.ReadAsync()) return null;

            return new()
            {
                UserId = reader.ReadGuid(0),
                ExternalIdentityId = reader.ReadGuid(1),
                SessionId = reader.ReadGuid(2),
                AuthPolicyId = reader.ReadGuid(3),
                AbsoluteExpires = reader.ReadDateTimeOffset(4),
                ReturnPath = reader.ReadString(5),
                Persist = reader.ReadBoolean(6) ?? false,
            };
        });
    }
}