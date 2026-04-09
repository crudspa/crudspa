namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PaneInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Pane pane)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PaneInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SegmentId", pane.SegmentId);
        command.AddParameter("@Title", 150, pane.Title);
        command.AddParameter("@Key", 75, pane.Key);
        command.AddParameter("@TypeId", pane.TypeId);
        command.AddParameter("@PermissionId", pane.PermissionId);
        command.AddParameter("@ConfigJson", pane.ConfigJson);
        command.AddParameter("@Ordinal", pane.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}