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
            SeoTitle = reader.ReadString(10),
            SeoDescription = reader.ReadString(11),
            SeoKeywords = reader.ReadString(12),
            SeoImageFile = new()
            {
                Id = reader.ReadGuid(13),
                BlobId = reader.ReadGuid(14),
                Name = reader.ReadString(15),
                Format = reader.ReadString(16),
                Width = reader.ReadInt32(17),
                Height = reader.ReadInt32(18),
                Caption = reader.ReadString(19),
            },
            CanonicalBaseUrl = reader.ReadString(20),
            Portal = new()
            {
                Id = reader.ReadGuid(21),
                Key = reader.ReadString(22),
                Title = reader.ReadString(23),
                SegmentCount = reader.ReadInt32(24),
            },
            FooterPageId = reader.ReadGuid(25),
            AchievementCount = reader.ReadInt32(26),
            BlogCount = reader.ReadInt32(27),
            ForumCount = reader.ReadInt32(28),
            TrackCount = reader.ReadInt32(29),
            StyleCount = reader.ReadInt32(30),
            FontCount = reader.ReadInt32(31),
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