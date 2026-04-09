namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadChoiceDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadChoice readChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadChoiceDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readChoice.Id);

        await command.Execute(connection, transaction);
    }
}