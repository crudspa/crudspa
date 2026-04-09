namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabQuestionDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabQuestion vocabQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabQuestionDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", vocabQuestion.Id);

        await command.Execute(connection, transaction);
    }
}