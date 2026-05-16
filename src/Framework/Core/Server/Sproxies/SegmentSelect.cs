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
            Routable = reader.ReadBoolean(12),
            Navigable = reader.ReadBoolean(13),
            Mapable = reader.ReadBoolean(14),
            RequiresId = reader.ReadBoolean(15),
            Recursive = reader.ReadBoolean(16),
            TypeId = reader.ReadGuid(17),
            TypeName = reader.ReadString(18),
            AllLicenses = reader.ReadBoolean(19),
            ParentId = reader.ReadGuid(20),
            Ordinal = reader.ReadInt32(21),
            SegmentCount = reader.ReadInt32(22),
            PaneCount = reader.ReadInt32(23),
        };
    }
}