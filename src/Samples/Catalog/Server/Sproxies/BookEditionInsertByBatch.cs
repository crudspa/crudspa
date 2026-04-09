namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class BookEditionInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, BookEdition bookEdition)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.BookEditionInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", bookEdition.BookId);
        command.AddParameter("@FormatId", bookEdition.FormatId);
        command.AddParameter("@Sku", 40, bookEdition.Sku);
        command.AddParameter("@Price", bookEdition.Price);
        command.AddParameter("@ReleasedOn", bookEdition.ReleasedOn);
        command.AddParameter("@InPrint", bookEdition.InPrint ?? true);
        command.AddParameter("@Ordinal", bookEdition.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}