namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitBookDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UnitBook unitBook)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitBookDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unitBook.Id);

        await command.Execute(connection, transaction);
    }
}