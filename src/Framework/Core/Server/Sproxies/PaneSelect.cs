namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PaneSelect
{
    public static async Task<Pane?> Execute(String connection, Guid? sessionId, Pane pane)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PaneSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pane.Id);

        return await command.ReadSingle(connection, ReadPane);
    }

    private static Pane ReadPane(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SegmentId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            Key = reader.ReadString(3),
            TypeId = reader.ReadGuid(4),
            TypeName = reader.ReadString(5),
            TypeEditorView = reader.ReadString(6),
            PermissionId = reader.ReadGuid(7),
            PermissionName = reader.ReadString(8),
            ConfigJson = reader.ReadString(9),
            Ordinal = reader.ReadInt32(10),
        };
    }
}