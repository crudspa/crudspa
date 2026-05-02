namespace Crudspa.Content.Display.Server.Sproxies;

public static class ContentPortalSelect
{
    public static async Task<ContentPortal?> Execute(String connection, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ContentPortalSelect";

        command.AddParameter("@Id", portalId);

        return await command.ReadSingle(connection, ReadPortal);
    }

    private static ContentPortal ReadPortal(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Portal = new()
            {
                Id = reader.ReadGuid(1),
                Key = reader.ReadString(2),
                Title = reader.ReadString(3),
                SessionsPersist = reader.ReadBoolean(4),
                AllowSignIn = reader.ReadBoolean(5),
                RequireSignIn = reader.ReadBoolean(6),
                NavigationTypeDisplayView = reader.ReadString(7),
            },
            MaxWidth = reader.ReadString(8),
            BrandingImageFile = new()
            {
                Id = reader.ReadGuid(9),
                BlobId = reader.ReadGuid(10),
                Name = reader.ReadString(11),
                Format = reader.ReadString(12),
                Width = reader.ReadInt32(13),
                Height = reader.ReadInt32(14),
                Caption = reader.ReadString(15),
            },
            SeoTitle = reader.ReadString(16),
            SeoDescription = reader.ReadString(17),
            SeoKeywords = reader.ReadString(18),
            SeoImageFile = new()
            {
                Id = reader.ReadGuid(19),
                BlobId = reader.ReadGuid(20),
                Name = reader.ReadString(21),
                Format = reader.ReadString(22),
                Width = reader.ReadInt32(23),
                Height = reader.ReadInt32(24),
                Caption = reader.ReadString(25),
            },
            CanonicalBaseUrl = reader.ReadString(26),
            FooterPageId = reader.ReadGuid(27),
        };
    }
}