namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class CatalogContactUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CatalogContact catalogContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogContactUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", catalogContact.Id);
        command.AddParameter("@UserId", catalogContact.UserId);

        await command.Execute(connection, transaction);
    }
}