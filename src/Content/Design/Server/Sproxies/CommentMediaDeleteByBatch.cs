namespace Crudspa.Content.Design.Server.Sproxies;

public static class CommentMediaDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CommentMedia commentMedia)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CommentMediaDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", commentMedia.Id);

        await command.Execute(connection, transaction);
    }
}