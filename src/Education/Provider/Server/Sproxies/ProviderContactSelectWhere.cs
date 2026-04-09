namespace Crudspa.Education.Provider.Server.Sproxies;

public static class ProviderContactSelectWhere
{
    public static async Task<IList<ProviderContact>> Execute(String connection, Guid? sessionId, ProviderContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.ProviderContactSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadProviderContact);
    }

    private static ProviderContact ReadProviderContact(SqlDataReader reader)
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