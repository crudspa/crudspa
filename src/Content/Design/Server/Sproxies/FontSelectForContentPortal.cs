namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontSelectForContentPortal
{
    public static async Task<IList<Font>> Execute(String connection, Guid? sessionId, Guid? contentPortalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontSelectForContentPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContentPortalId", contentPortalId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var fonts = new List<Font>();
            var fontsById = new Dictionary<Guid, Font>();

            while (await reader.ReadAsync())
            {
                var font = ReadFont(reader);
                fonts.Add(font);

                if (font.Id.HasValue)
                    fontsById[font.Id.Value] = font;
            }

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                var fontFace = ReadFontFace(reader);

                if (fontFace.FontId.HasValue && fontsById.TryGetValue(fontFace.FontId.Value, out var font))
                    font.Faces.Add(fontFace);
            }

            return fonts;
        });
    }

    private static Font ReadFont(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContentPortalId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
        };
    }

    private static FontFace ReadFontFace(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            FontId = reader.ReadGuid(1),
            FileFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
                Description = reader.ReadString(6),
            },
            Style = reader.ReadString(7),
            WeightMin = reader.ReadInt32(8),
            WeightMax = reader.ReadInt32(9),
        };
    }
}