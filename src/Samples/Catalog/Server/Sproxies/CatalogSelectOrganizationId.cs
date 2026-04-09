namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class CatalogSelectOrganizationId
{
    public static async Task<Guid?> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogSelectOrganizationId";

        return await command.ReadSingle(connection, reader => reader.ReadGuid(0));
    }
}