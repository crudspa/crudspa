namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentSelectStructure
{
    public static async Task<Segment?> Execute(String connection, Guid? sessionId, Segment segment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentSelectStructure";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segment.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            segment = ReadSegment(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                segment.Panes.Add(ReadPane(reader));

            return segment;
        });
    }

    private static Segment ReadSegment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            TypeId = reader.ReadGuid(1),
            ConfigJson = reader.ReadString(2),
            TypeName = reader.ReadString(3),
            TypeDisplayView = reader.ReadString(4),
            TypeEditorView = reader.ReadString(5),
        };
    }

    private static Pane ReadPane(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Key = reader.ReadString(1),
            Title = reader.ReadString(2),
            SegmentId = reader.ReadGuid(3),
            TypeId = reader.ReadGuid(4),
            PermissionId = reader.ReadGuid(5),
            ConfigJson = reader.ReadString(6),
            Ordinal = reader.ReadInt32(7),
            TypeName = reader.ReadString(8),
        };
    }
}