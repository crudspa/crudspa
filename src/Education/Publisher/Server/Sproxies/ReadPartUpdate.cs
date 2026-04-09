namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadPartUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadPart readPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadPartUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readPart.Id);
        command.AddParameter("@Title", 100, readPart.Title);
        command.AddParameter("@PassageInstructionsText", readPart.PassageInstructionsText);
        command.AddParameter("@PassageInstructionsAudioFileId", readPart.PassageInstructionsAudioFileFile.Id);
        command.AddParameter("@PreviewInstructionsText", readPart.PreviewInstructionsText);
        command.AddParameter("@PreviewInstructionsAudioFileId", readPart.PreviewInstructionsAudioFileFile.Id);
        command.AddParameter("@QuestionsInstructionsText", readPart.QuestionsInstructionsText);
        command.AddParameter("@QuestionsInstructionsAudioFileId", readPart.QuestionsInstructionsAudioFileFile.Id);

        await command.Execute(connection, transaction);
    }
}