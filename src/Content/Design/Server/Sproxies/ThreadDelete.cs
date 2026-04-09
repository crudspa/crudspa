using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class ThreadDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Thread thread)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ThreadDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", thread.Id);

        await command.Execute(connection, transaction);
    }
}