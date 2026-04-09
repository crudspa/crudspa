namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenPartUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenPart listenPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenPartUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", listenPart.Id);
        command.AddParameter("@Title", 100, listenPart.Title);
        command.AddParameter("@PassageAudioFileId", listenPart.PassageAudioFileFile.Id);
        command.AddParameter("@PassageInstructionsText", listenPart.PassageInstructionsText);
        command.AddParameter("@PassageInstructionsAudioFileId", listenPart.PassageInstructionsAudioFileFile.Id);
        command.AddParameter("@PreviewInstructionsText", listenPart.PreviewInstructionsText);
        command.AddParameter("@PreviewInstructionsAudioFileId", listenPart.PreviewInstructionsAudioFileFile.Id);
        command.AddParameter("@QuestionsInstructionsText", listenPart.QuestionsInstructionsText);
        command.AddParameter("@QuestionsInstructionsAudioFileId", listenPart.QuestionsInstructionsAudioFileFile.Id);

        await command.Execute(connection, transaction);
    }
}