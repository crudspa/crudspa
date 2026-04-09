namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookSelectRelations
{
    public const String Grades = "Grades";
    public const String SchoolYears = "School Years";

    public static async Task<Book> Execute(String connection, Guid? sessionId, Book book)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookSelectRelations";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", book.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var relation = new RelatedEntity { Name = Grades };

            while (await reader.ReadAsync())
                relation.Selections.Add(reader.ReadSelectable());

            book.Relations.Add(relation);

            await reader.NextResultAsync();

            relation = new() { Name = SchoolYears };

            while (await reader.ReadAsync())
                relation.Selections.Add(reader.ReadSelectable());

            book.Relations.Add(relation);

            return book;
        });
    }
}