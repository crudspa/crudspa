using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class ThreadInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Thread thread)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ThreadInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ForumId", thread.ForumId);
        command.AddParameter("@Title", 150, thread.Title);
        command.AddParameter("@Pinned", thread.Pinned ?? false);
        command.AddParameter("@CommentBody", thread.Comment.Body);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}