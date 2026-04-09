namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class BookInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Book book)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.BookInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Isbn", 20, book.Isbn);
        command.AddParameter("@Title", 160, book.Title);
        command.AddParameter("@Author", 120, book.Author);
        command.AddParameter("@GenreId", book.GenreId);
        command.AddParameter("@Pages", book.Pages);
        command.AddParameter("@Price", book.Price);
        command.AddParameter("@Summary", book.Summary);
        command.AddParameter("@CoverImageId", book.CoverImageFile.Id);
        command.AddParameter("@SamplePdfId", book.SamplePdfFile.Id);
        command.AddParameter("@PreviewAudioFileId", book.PreviewAudioFileFile.Id);
        command.AddParameter("@Tags", book.Tags);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}