namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class CatalogContactDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CatalogContact catalogContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogContactDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", catalogContact.Id);

        await command.Execute(connection, transaction);
    }
}