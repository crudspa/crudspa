namespace Crudspa.Content.Design.Server.Sproxies;

public static class PostSelect
{
    public static async Task<Post?> Execute(String connection, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PostSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", post.Id);

        return await command.ReadSingle(connection, ReadPost);
    }

    private static Post ReadPost(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BlogId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            StatusId = reader.ReadGuid(3),
            StatusName = reader.ReadString(4),
            Author = reader.ReadString(5),
            Published = reader.ReadDateOnly(6),
            Revised = reader.ReadDateOnly(7),
            CommentRule = reader.ReadEnum<Post.CommentRules>(8),
            Page = new()
            {
                Id = reader.ReadGuid(9),
                TypeId = reader.ReadGuid(10),
                TypeName = reader.ReadString(11),
            },
            CommentCount = reader.ReadInt32(12),
            PostReactionCount = reader.ReadInt32(13),
            PostTagCount = reader.ReadInt32(14),
            SectionCount = reader.ReadInt32(15),
        };
    }
}