namespace Crudspa.Content.Design.Server.Sproxies;

public static class MultimediaItemDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, MultimediaItem multimediaItem)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MultimediaItemDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", multimediaItem.Id);

        await command.Execute(connection, transaction);
    }
}