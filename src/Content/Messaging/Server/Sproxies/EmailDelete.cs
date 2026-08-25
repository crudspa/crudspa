namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class EmailDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Email email)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.EmailDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", email.Id);

        await command.Execute(connection, transaction);
    }
}