namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabPartInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabPart vocabPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabPartInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", vocabPart.AssessmentId);
        command.AddParameter("@Title", 100, vocabPart.Title);
        command.AddParameter("@PreviewInstructionsText", vocabPart.PreviewInstructionsText);
        command.AddParameter("@PreviewInstructionsAudioFileId", vocabPart.PreviewInstructionsAudioFileFile.Id);
        command.AddParameter("@QuestionsInstructionsText", vocabPart.QuestionsInstructionsText);
        command.AddParameter("@QuestionsInstructionsAudioFileId", vocabPart.QuestionsInstructionsAudioFileFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}