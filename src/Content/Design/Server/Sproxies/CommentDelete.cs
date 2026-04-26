using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Design.Server.Sproxies;

public static class CommentDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Comment comment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CommentDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", comment.Id);

        await command.Execute(connection, transaction);
    }
}