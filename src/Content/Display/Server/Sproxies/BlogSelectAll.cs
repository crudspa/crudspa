namespace Crudspa.Content.Display.Server.Sproxies;

public static class BlogSelectAll
{
    public static async Task<IList<Blog>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.BlogSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadBlog);
    }

    private static Blog ReadBlog(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            StatusId = reader.ReadGuid(2),
            Title = reader.ReadString(3),
            Author = reader.ReadString(4),
            Description = reader.ReadString(5),
            ImageFile = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                Width = reader.ReadInt32(10),
                Height = reader.ReadInt32(11),
                Caption = reader.ReadString(12),
            },
            StatusName = reader.ReadString(12),
            PostCount = reader.ReadInt32(13),
        };
    }
}