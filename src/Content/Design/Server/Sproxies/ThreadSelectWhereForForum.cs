using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class ThreadSelectWhereForForum
{
    public static async Task<IList<Thread>> Execute(String connection, Guid? sessionId, ThreadSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ThreadSelectWhereForForum";

        search.PostedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ForumId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@PostedStart", search.PostedRange.StartDateTimeOffset);
        command.AddParameter("@PostedEnd", search.PostedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadThread);
    }

    private static Thread ReadThread(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            ForumId = reader.ReadGuid(3),
            ForumTitle = reader.ReadString(4),
            Title = reader.ReadString(5),
            Pinned = reader.ReadBoolean(6),
            Comment = new()
            {
                Id = reader.ReadGuid(7),
                Body = reader.ReadString(8),
                ById = reader.ReadGuid(9),
                ByName = reader.ReadString(10),
                Posted = reader.ReadDateTimeOffset(11),
                Edited = reader.ReadDateTimeOffset(12),
            },
            CommentCount = reader.ReadInt32(13),
        };
    }
}