namespace Crudspa.Content.Display.Server.Sproxies;

public static class BlogSelectRun
{
    public static async Task<Blog?> Execute(String connection, Guid? blogId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.BlogSelectRun";

        command.AddParameter("@Id", blogId);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var blog = ReadBlog(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                blog.Posts.Add(ReadPost(reader));

            return blog;
        });
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
            StatusName = reader.ReadString(13),
            PostCount = reader.ReadInt32(14),
        };
    }

    private static Post ReadPost(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BlogId = reader.ReadGuid(1),
            StatusId = reader.ReadGuid(2),
            Title = reader.ReadString(3),
            Author = reader.ReadString(4),
            Published = reader.ReadDateOnly(5),
            Revised = reader.ReadDateOnly(6),
            CommentRule = reader.ReadEnum<Post.CommentRules>(7),
            StatusName = reader.ReadString(8),
            SectionCount = reader.ReadInt32(9),
            CommentCount = reader.ReadInt32(10),
            PostReactionCount = reader.ReadInt32(11),
            PostTagCount = reader.ReadInt32(12),
        };
    }
}