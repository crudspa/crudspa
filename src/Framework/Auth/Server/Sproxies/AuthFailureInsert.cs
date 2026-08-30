using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthFailureInsert
{
    public static async Task Execute(String connection, Guid correlationId, String provider, String? audience, String reason)
    {
        await using var command = new SqlCommand { CommandText = "FrameworkAuth.AuthFailureInsert" };
        command.AddParameter("@CorrelationId", correlationId);
        command.AddParameter("@Provider", 75, provider);
        command.AddParameter("@Audience", 25, audience);
        command.AddParameter("@Reason", 75, reason);
        await command.Execute(connection);
    }
}