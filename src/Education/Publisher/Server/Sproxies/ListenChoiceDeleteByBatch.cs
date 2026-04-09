namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenChoiceDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenChoice listenChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenChoiceDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", listenChoice.Id);

        await command.Execute(connection, transaction);
    }
}