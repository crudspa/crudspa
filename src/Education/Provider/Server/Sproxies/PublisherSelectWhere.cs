namespace Crudspa.Education.Provider.Server.Sproxies;

public static class PublisherSelectWhere
{
    public static async Task<IList<Publisher>> Execute(String connection, Guid? sessionId, PublisherSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.PublisherSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadPublisher);
    }

    private static Publisher ReadPublisher(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            OrganizationId = reader.ReadGuid(3),
            PublisherContactCount = reader.ReadInt32(4),
        };
    }
}