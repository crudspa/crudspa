namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class CatalogContactInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CatalogContact catalogContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogContactInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", catalogContact.ContactId);
        command.AddParameter("@UserId", catalogContact.UserId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}