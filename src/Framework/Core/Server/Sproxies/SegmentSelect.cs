namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentSelect
{
    public static async Task<Segment?> Execute(String connection, Guid? sessionId, Segment segment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segment.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            segment = ReadSegment(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                segment.Licenses.Add(reader.ReadSelectable());

            return segment;
        });
    }

    private static Segment ReadSegment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            Key = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            SeoDescription = reader.ReadString(6),
            PermissionId = reader.ReadGuid(7),
            PermissionName = reader.ReadString(8),
            IconId = reader.ReadGuid(9),
            IconCssClass = reader.ReadString(10),
            Fixed = reader.ReadBoolean(11),
            RequiresId = reader.ReadBoolean(12),
            Recursive = reader.ReadBoolean(13),
            TypeId = reader.ReadGuid(14),
            TypeName = reader.ReadString(15),
            AllLicenses = reader.ReadBoolean(16),
            ParentId = reader.ReadGuid(17),
            Ordinal = reader.ReadInt32(18),
            SegmentCount = reader.ReadInt32(19),
            PaneCount = reader.ReadInt32(20),
        };
    }
}