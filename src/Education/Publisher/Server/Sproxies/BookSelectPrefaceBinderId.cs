namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookSelectPrefaceBinderId
{
    public static async Task<Book?> Execute(String connection, Guid? sessionId, Book book)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookSelectPrefaceBinderId";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", book.Id);

        return await command.ReadSingle(connection, ReadBook);
    }

    private static Book ReadBook(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PrefaceBinderId = reader.ReadGuid(1),
        };
    }
}