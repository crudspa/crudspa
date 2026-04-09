namespace Crudspa.Content.Design.Server.Sproxies;

public static class PaneSelectForPage
{
    public static async Task<Pane?> Execute(String connection, Guid? sessionId, Guid? pageId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PaneSelectForPage";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageId", pageId);

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