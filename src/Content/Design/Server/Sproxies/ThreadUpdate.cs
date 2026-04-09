using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class ThreadUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Thread thread)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ThreadUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", thread.Id);
        command.AddParameter("@Title", 150, thread.Title);
        command.AddParameter("@Pinned", thread.Pinned ?? false);
        command.AddParameter("@CommentBody", thread.Comment.Body);

        await command.Execute(connection, transaction);
    }
}