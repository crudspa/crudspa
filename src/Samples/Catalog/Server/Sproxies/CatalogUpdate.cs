namespace Crudspa.Samples.Catalog.Server.Sproxies;

using Catalog = Shared.Contracts.Data.Catalog;

public static class CatalogUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Catalog catalog)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", catalog.Id);

        await command.Execute(connection, transaction);
    }
}