namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class CatalogContactSelectWhere
{
    public static async Task<IList<CatalogContact>> Execute(String connection, Guid? sessionId, CatalogContactSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogContactSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);

        return await command.ReadAll(connection, ReadCatalogContact);
    }

    private static CatalogContact ReadCatalogContact(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            ContactId = reader.ReadGuid(3),
            UserId = reader.ReadGuid(4),
        };
    }
}