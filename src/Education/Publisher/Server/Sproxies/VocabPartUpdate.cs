namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabPartUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabPart vocabPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabPartUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", vocabPart.Id);
        command.AddParameter("@Title", 100, vocabPart.Title);
        command.AddParameter("@PreviewInstructionsText", vocabPart.PreviewInstructionsText);
        command.AddParameter("@PreviewInstructionsAudioFileId", vocabPart.PreviewInstructionsAudioFileFile.Id);
        command.AddParameter("@QuestionsInstructionsText", vocabPart.QuestionsInstructionsText);
        command.AddParameter("@QuestionsInstructionsAudioFileId", vocabPart.QuestionsInstructionsAudioFileFile.Id);

        await command.Execute(connection, transaction);
    }
}