namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtOptionUpdateRelations
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ShirtOption shirtOption)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtOptionUpdateRelations";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", shirtOption.Id);
        command.AddParameter("@AllSizes", shirtOption.AllSizes ?? true);
        command.AddParameter("@Sizes", shirtOption.Sizes);

        await command.Execute(connection, transaction);
    }
}