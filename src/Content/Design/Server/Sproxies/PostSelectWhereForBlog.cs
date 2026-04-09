namespace Crudspa.Content.Design.Server.Sproxies;

public static class PostSelectWhereForBlog
{
    public static async Task<IList<Post>> Execute(String connection, Guid? sessionId, PostSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PostSelectWhereForBlog";

        search.PublishedRange.ResolveDates(search.TimeZoneId!);
        search.RevisedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BlogId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Status", search.Status);
        command.AddParameter("@PublishedStart", search.PublishedRange.StartDateTimeOffset);
        command.AddParameter("@PublishedEnd", search.PublishedRange.EndDateTimeOffset);
        command.AddParameter("@RevisedStart", search.RevisedRange.StartDateTimeOffset);
        command.AddParameter("@RevisedEnd", search.RevisedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadPost);
    }

    private static Post ReadPost(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            BlogId = reader.ReadGuid(3),
            Title = reader.ReadString(4),
            StatusId = reader.ReadGuid(5),
            StatusName = reader.ReadString(6),
            Author = reader.ReadString(7),
            Published = reader.ReadDateOnly(8),
            Revised = reader.ReadDateOnly(9),
            CommentRule = reader.ReadEnum<Post.CommentRules>(10),
            Page = new()
            {
                Id = reader.ReadGuid(11),
                TypeId = reader.ReadGuid(12),
                TypeName = reader.ReadString(13),
            },
            CommentCount = reader.ReadInt32(14),
            PostReactionCount = reader.ReadInt32(15),
            PostTagCount = reader.ReadInt32(16),
            SectionCount = reader.ReadInt32(17),
        };
    }
}