namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PaneDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Pane pane)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PaneDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pane.Id);

        await command.Execute(connection, transaction);
    }
}