namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabChoiceUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabChoice vocabChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabChoiceUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", vocabChoice.Id);
        command.AddParameter("@VocabQuestionId", vocabChoice.VocabQuestionId);
        command.AddParameter("@Word", 50, vocabChoice.Word);
        command.AddParameter("@IsCorrect", vocabChoice.IsCorrect ?? false);
        command.AddParameter("@AudioFileId", vocabChoice.AudioFileFile.Id);
        command.AddParameter("@Ordinal", vocabChoice.Ordinal);

        await command.Execute(connection, transaction);
    }
}