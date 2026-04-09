namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Book book)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Title", 150, book.Title);
        command.AddParameter("@StatusId", book.StatusId);
        command.AddParameter("@Key", 100, book.Key);
        command.AddParameter("@Author", 150, book.Author);
        command.AddParameter("@Isbn", 20, book.Isbn);
        command.AddParameter("@Lexile", 10, book.Lexile);
        command.AddParameter("@SeasonId", book.SeasonId);
        command.AddParameter("@CategoryId", book.CategoryId);
        command.AddParameter("@RequiresAchievementId", book.RequiresAchievementId);
        command.AddParameter("@Summary", book.Summary);
        command.AddParameter("@CoverImageId", book.CoverImageFile.Id);
        command.AddParameter("@GuideImageId", book.GuideImageFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}