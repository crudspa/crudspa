namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookUpdateRelations
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Book book)
    {
        var grades = book.Relations.First(x => x.Name.IsBasically(BookSelectRelations.Grades)).Selections;
        var schoolYears = book.Relations.First(x => x.Name.IsBasically(BookSelectRelations.SchoolYears)).Selections;

        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookUpdateRelations";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", book.Id);
        command.AddParameter("@Grades", grades);
        command.AddParameter("@SchoolYears", schoolYears);

        await command.Execute(connection, transaction);
    }
}