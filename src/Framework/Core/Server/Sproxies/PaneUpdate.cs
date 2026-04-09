namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PaneUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Pane pane)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PaneUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pane.Id);
        command.AddParameter("@Title", 150, pane.Title);
        command.AddParameter("@Key", 75, pane.Key);
        command.AddParameter("@TypeId", pane.TypeId);
        command.AddParameter("@PermissionId", pane.PermissionId);
        command.AddParameter("@Ordinal", pane.Ordinal);

        await command.Execute(connection, transaction);
    }
}