namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabChoiceDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabChoice vocabChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabChoiceDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", vocabChoice.Id);

        await command.Execute(connection, transaction);
    }
}