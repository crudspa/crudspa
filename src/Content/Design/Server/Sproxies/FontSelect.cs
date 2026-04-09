namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontSelect
{
    public static async Task<Font?> Execute(String connection, Guid? sessionId, Font font)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", font.Id);

        return await command.ReadSingle(connection, ReadFont);
    }

    private static Font ReadFont(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContentPortalId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            FileFile = new()
            {
                Id = reader.ReadGuid(3),
                BlobId = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Format = reader.ReadString(6),
                Description = reader.ReadString(7),
            },
        };
    }
}