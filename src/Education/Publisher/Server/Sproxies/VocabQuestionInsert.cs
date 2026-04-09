namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabQuestionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabQuestion vocabQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabQuestionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@VocabPartId", vocabQuestion.VocabPartId);
        command.AddParameter("@Word", 50, vocabQuestion.Word);
        command.AddParameter("@AudioFileId", vocabQuestion.AudioFileFile.Id);
        command.AddParameter("@IsPreview", vocabQuestion.IsPreview ?? false);
        command.AddParameter("@PageBreakBefore", vocabQuestion.PageBreakBefore ?? false);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}