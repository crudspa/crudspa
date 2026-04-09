namespace Crudspa.Samples.Composer.Server.Sproxies;

public static class ComposerContactSelectWhere
{
    public static async Task<IList<ComposerContact>> Execute(String connection, Guid? sessionId, ComposerContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesComposer.ComposerContactSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadComposerContact);
    }

    private static ComposerContact ReadComposerContact(SqlDataReader reader)
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