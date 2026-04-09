namespace Crudspa.Content.Design.Server.Sproxies;

public static class TokenCreateMoreSignups
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Membership membership)
    {
        await using var command = new SqlCommand();
        command.CommandText = "PortalsProvider.TokensUpdateMoreSignups";

        command.AddParameter("@SessionId", sessionId);

        await command.Execute(connection, transaction);
    }
}