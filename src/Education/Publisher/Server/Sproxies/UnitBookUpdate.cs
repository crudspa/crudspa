namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitBookUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UnitBook unitBook)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitBookUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unitBook.Id);
        command.AddParameter("@BookId", unitBook.BookId);
        command.AddParameter("@Treatment", unitBook.Treatment ?? true);
        command.AddParameter("@Control", unitBook.Control ?? true);

        await command.Execute(connection, transaction);
    }
}