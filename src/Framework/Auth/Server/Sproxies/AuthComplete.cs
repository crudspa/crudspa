using Crudspa.Framework.Auth.Server.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthComplete
{
    public static async Task<AuthCompletion> Execute(
        String connection,
        Guid transactionId,
        String provider,
        String issuer,
        String subject,
        String tenant,
        String? providerRole,
        String providerAudience,
        Byte[] identityKeyHash,
        Guid handoffId,
        Byte[] codeHash)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkAuth.AuthComplete";

        command.AddParameter("@TransactionId", transactionId);
        command.AddParameter("@Provider", 75, provider);
        command.AddParameter("@Issuer", 500, issuer);
        command.AddParameter("@Subject", 255, subject);
        command.AddParameter("@Tenant", 255, tenant);
        command.AddParameter("@ProviderRole", 50, providerRole);
        command.AddParameter("@ProviderAudience", 25, providerAudience);
        command.AddParameter("@IdentityKeyHash", identityKeyHash);
        command.AddParameter("@HandoffId", handoffId);
        command.AddParameter("@CodeHash", codeHash);

        return await command.ExecuteQuery<AuthCompletion>(connection, async reader =>
        {
            await reader.ReadAsync();

            return new()
            {
                Code = (AuthCompletion.Codes)reader.GetInt32(0),
                PortalId = reader.ReadGuid(1),
                UserId = reader.ReadGuid(2),
                ReturnPath = reader.ReadString(3),
                Audience = reader.ReadString(4),
            };
        });
    }
}