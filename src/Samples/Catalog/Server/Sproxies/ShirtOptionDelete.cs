namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtOptionDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ShirtOption shirtOption)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtOptionDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", shirtOption.Id);

        await command.Execute(connection, transaction);
    }
}