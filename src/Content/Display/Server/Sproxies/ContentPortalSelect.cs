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
            FooterPageId = reader.ReadGuid(16),
        };
    }
}