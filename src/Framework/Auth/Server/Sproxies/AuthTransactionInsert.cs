using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Sproxies;

public static class AuthTransactionInsert
{
    public static async Task Execute(String connection, Guid id, String provider, String audience, String returnPath)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkAuth.AuthTransactionInsert";

        command.AddParameter("@Id", id);
        command.AddParameter("@Provider", 75, provider);
        command.AddParameter("@Audience", 25, audience);
        command.AddParameter("@ReturnPath", 500, returnPath);

        await command.Execute(connection);
    }
}