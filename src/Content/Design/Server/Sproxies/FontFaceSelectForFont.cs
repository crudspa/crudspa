namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontFaceSelectForFont
{
    public static async Task<IList<FontFace>> Execute(String connection, Guid? sessionId, Guid? fontId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontFaceSelectForFont";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@FontId", fontId);

        return await command.ReadAll(connection, ReadFontFace);
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