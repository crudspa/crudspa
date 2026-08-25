namespace Crudspa.Content.Design.Server.Sproxies;

public static class BlogSelect
{
    public static async Task<Blog?> Execute(String connection, Guid? sessionId, Blog blog)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BlogSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", blog.Id);

        return await command.ReadSingle(connection, ReadBlog);
    }

    private static Blog ReadBlog(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            StatusId = reader.ReadGuid(3),
            StatusName = reader.ReadString(4),
            Author = reader.ReadString(5),
            Description = reader.ReadString(6),
            AccessMode = reader.ReadEnum<Blog.AccessModes>(7),
            ImageFile = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                Width = reader.ReadInt32(12),
                Height = reader.ReadInt32(13),
                Caption = reader.ReadString(14),
            },
            PostCount = reader.ReadInt32(15),
        };
    }
}