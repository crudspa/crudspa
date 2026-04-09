namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabChoiceInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabChoice vocabChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabChoiceInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@VocabQuestionId", vocabChoice.VocabQuestionId);
        command.AddParameter("@Word", 50, vocabChoice.Word);
        command.AddParameter("@IsCorrect", vocabChoice.IsCorrect ?? false);
        command.AddParameter("@AudioFileId", vocabChoice.AudioFileFile.Id);
        command.AddParameter("@Ordinal", vocabChoice.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}