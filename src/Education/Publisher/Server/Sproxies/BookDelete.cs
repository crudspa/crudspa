namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Book book)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", book.Id);

        await command.Execute(connection, transaction);
    }
}