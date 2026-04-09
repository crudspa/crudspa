namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class BookUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Book book)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.BookUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", book.Id);
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

        await command.Execute(connection, transaction);
    }
}