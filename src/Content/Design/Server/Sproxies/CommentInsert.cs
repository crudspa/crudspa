using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Design.Server.Sproxies;

public static class CommentInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Comment comment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CommentInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PostId", comment.PostId);
        command.AddParameter("@ThreadId", comment.ThreadId);
        command.AddParameter("@ParentId", comment.ParentId);
        command.AddParameter("@Body", comment.Body);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}