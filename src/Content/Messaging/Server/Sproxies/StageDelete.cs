namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class StageDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Stage stage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.StageDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", stage.Id);

        await command.Execute(connection, transaction);
    }
}