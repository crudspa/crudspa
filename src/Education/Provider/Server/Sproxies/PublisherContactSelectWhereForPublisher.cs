namespace Crudspa.Education.Provider.Server.Sproxies;

public static class PublisherContactSelectWhereForPublisher
{
    public static async Task<IList<PublisherContact>> Execute(String connection, Guid? sessionId, PublisherContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.PublisherContactSelectWhereForPublisher";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PublisherId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadPublisherContact);
    }

    private static PublisherContact ReadPublisherContact(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            PublisherId = reader.ReadGuid(3),
            UserId = reader.ReadGuid(4),
            ContactId = reader.ReadGuid(5),
        };
    }
}