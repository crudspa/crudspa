namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitBookInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UnitBook unitBook)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitBookInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@UnitId", unitBook.UnitId);
        command.AddParameter("@BookId", unitBook.BookId);
        command.AddParameter("@Treatment", unitBook.Treatment ?? true);
        command.AddParameter("@Control", unitBook.Control ?? true);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}