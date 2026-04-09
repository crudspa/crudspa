namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Shirt shirt)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", shirt.Id);

        await command.Execute(connection, transaction);
    }
}