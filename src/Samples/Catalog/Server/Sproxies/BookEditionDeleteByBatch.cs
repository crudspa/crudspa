namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class BookEditionDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, BookEdition bookEdition)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.BookEditionDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", bookEdition.Id);

        await command.Execute(connection, transaction);
    }
}