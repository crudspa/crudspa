namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PublisherContactSelectWhere
{
    public static async Task<IList<PublisherContact>> Execute(String connection, Guid? sessionId, PublisherContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PublisherContactSelectWhere";

        command.AddParameter("@SessionId", sessionId);
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
            UserId = reader.ReadGuid(3),
            ContactId = reader.ReadGuid(4),
        };
    }
}