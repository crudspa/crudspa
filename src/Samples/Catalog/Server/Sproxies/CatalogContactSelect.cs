namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class CatalogContactSelect
{
    public static async Task<CatalogContact?> Execute(String connection, Guid? sessionId, CatalogContact catalogContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", catalogContact.Id);

        return await command.ReadSingle(connection, ReadCatalogContact);
    }

    private static CatalogContact ReadCatalogContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContactId = reader.ReadGuid(1),
            UserId = reader.ReadGuid(2),
        };
    }
}