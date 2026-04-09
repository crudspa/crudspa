namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadPartDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadPart readPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadPartDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readPart.Id);

        await command.Execute(connection, transaction);
    }
}