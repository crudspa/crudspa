namespace Crudspa.Content.Display.Server.Sproxies;

public static class PostSelectRun
{
    public static async Task<Post?> Execute(String connection, Post post, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.PostSelectRun";

        command.AddParameter("@Id", post.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadPost);
    }

    private static Post ReadPost(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BlogId = reader.ReadGuid(1),
            PageId = reader.ReadGuid(2),
            StatusId = reader.ReadGuid(3),
            Title = reader.ReadString(4),
            Author = reader.ReadString(5),
            Published = reader.ReadDateOnly(6),
            Revised = reader.ReadDateOnly(7),
            CommentRule = reader.ReadEnum<Post.CommentRules>(8),
            StatusName = reader.ReadString(9),
            SectionCount = reader.ReadInt32(10),
            CommentCount = reader.ReadInt32(11),
            PostReactionCount = reader.ReadInt32(12),
            PostTagCount = reader.ReadInt32(13),
        };
    }
}