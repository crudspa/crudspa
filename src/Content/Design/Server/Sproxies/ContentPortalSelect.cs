namespace Crudspa.Content.Design.Server.Sproxies;

public static class ContentPortalSelect
{
    public static async Task<ContentPortal?> Execute(String connection, Guid? sessionId, ContentPortal contentPortal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ContentPortalSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contentPortal.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var portal = ReadContentPortal(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                portal.Portal.Features.Add(ReadPortalFeature(reader));

            return portal;
        });
    }

    private static ContentPortal ReadContentPortal(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MaxWidth = reader.ReadString(1),
            StyleRevision = reader.ReadInt32(2),
            BrandingImageFile = new()
            {
                Id = reader.ReadGuid(3),
                BlobId = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Format = reader.ReadString(6),
                Width = reader.ReadInt32(7),
                Height = reader.ReadInt32(8),
                Caption = reader.ReadString(9),
            },
            Portal = new()
            {
                Id = reader.ReadGuid(10),
                Key = reader.ReadString(11),
                Title = reader.ReadString(12),
                SegmentCount = reader.ReadInt32(13),
            },
            FooterPageId = reader.ReadGuid(14),
            AchievementCount = reader.ReadInt32(15),
            BlogCount = reader.ReadInt32(16),
            ForumCount = reader.ReadInt32(17),
            TrackCount = reader.ReadInt32(18),
            StyleCount = reader.ReadInt32(19),
            FontCount = reader.ReadInt32(20),
        };
    }

    private static PortalFeature ReadPortalFeature(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Key = reader.ReadString(2),
            Title = reader.ReadString(3),
            IconId = reader.ReadGuid(4),
            PermissionId = reader.ReadGuid(5),
            IconCssClass = reader.ReadString(6),
        };
    }
}