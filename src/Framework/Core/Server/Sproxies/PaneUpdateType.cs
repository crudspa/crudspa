namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PaneUpdateType
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Pane pane)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PaneUpdateType";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pane.Id);
        command.AddParameter("@TypeId", pane.TypeId);
        command.AddParameter("@ConfigJson", pane.ConfigJson);

        await command.Execute(connection, transaction);
    }
}