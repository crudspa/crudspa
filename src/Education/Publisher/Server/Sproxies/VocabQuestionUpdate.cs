namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabQuestionUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabQuestion vocabQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabQuestionUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", vocabQuestion.Id);
        command.AddParameter("@Word", 50, vocabQuestion.Word);
        command.AddParameter("@AudioFileId", vocabQuestion.AudioFileFile.Id);
        command.AddParameter("@IsPreview", vocabQuestion.IsPreview ?? false);
        command.AddParameter("@PageBreakBefore", vocabQuestion.PageBreakBefore ?? false);

        await command.Execute(connection, transaction);
    }
}