namespace Crudspa.Framework.Core.Server.Sproxies;

public static class LinkFollowedInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, String? url)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.LinkFollowedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Url", 250, url);

        await command.Execute(connection, transaction);
    }
}