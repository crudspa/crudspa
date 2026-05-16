namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontSelect
{
    public static async Task<Font?> Execute(String connection, Guid? sessionId, Font font)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", font.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var font = ReadFont(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                font.Faces.Add(ReadFontFace(reader));

            return font;
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