namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontSelectForContentPortal
{
    public static async Task<IList<Font>> Execute(String connection, Guid? sessionId, Guid? contentPortalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontSelectForContentPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContentPortalId", contentPortalId);

        return await command.ReadAll(connection, ReadFont);
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