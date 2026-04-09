namespace Crudspa.Samples.Catalog.Server.Sproxies;

using Catalog = Shared.Contracts.Data.Catalog;

public static class CatalogSelect
{
    public static async Task<Catalog?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogSelect";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadCatalog);
    }

    private static Catalog ReadCatalog(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
        };
    }
}