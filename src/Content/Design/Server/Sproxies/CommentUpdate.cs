namespace Crudspa.Content.Design.Server.Sproxies;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;

public static class CommentUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Comment comment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CommentUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", comment.Id);
        command.AddParameter("@Body", comment.Body);

        await command.Execute(connection, transaction);
    }
}