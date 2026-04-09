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
            PermissionId = reader.ReadGuid(6),
            PermissionName = reader.ReadString(7),
            IconId = reader.ReadGuid(8),
            IconCssClass = reader.ReadString(9),
            Fixed = reader.ReadBoolean(10),
            RequiresId = reader.ReadBoolean(11),
            Recursive = reader.ReadBoolean(12),
            TypeId = reader.ReadGuid(13),
            TypeName = reader.ReadString(14),
            AllLicenses = reader.ReadBoolean(15),
            ParentId = reader.ReadGuid(16),
            Ordinal = reader.ReadInt32(17),
            SegmentCount = reader.ReadInt32(18),
            PaneCount = reader.ReadInt32(19),
        };
    }
}