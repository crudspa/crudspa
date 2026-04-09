namespace Crudspa.Content.Design.Server.Sproxies;

public static class ContentPortalSelectAll
{
    public static async Task<IList<ContentPortal>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ContentPortalSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var portals = new List<ContentPortal>();

            while (await reader.ReadAsync())
                portals.Add(ReadContentPortal(reader));

            var features = new List<PortalFeature>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                features.Add(ReadPortalFeature(reader));

            foreach (var contentPortal in portals)
                contentPortal.Portal.Features = features.Where(x => x.PortalId.Equals(contentPortal.Portal.Id)).ToObservable();

            return portals;
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