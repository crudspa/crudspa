namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenPartInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenPart listenPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenPartInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", listenPart.AssessmentId);
        command.AddParameter("@Title", 100, listenPart.Title);
        command.AddParameter("@PassageAudioFileId", listenPart.PassageAudioFileFile.Id);
        command.AddParameter("@PassageInstructionsText", listenPart.PassageInstructionsText);
        command.AddParameter("@PassageInstructionsAudioFileId", listenPart.PassageInstructionsAudioFileFile.Id);
        command.AddParameter("@PreviewInstructionsText", listenPart.PreviewInstructionsText);
        command.AddParameter("@PreviewInstructionsAudioFileId", listenPart.PreviewInstructionsAudioFileFile.Id);
        command.AddParameter("@QuestionsInstructionsText", listenPart.QuestionsInstructionsText);
        command.AddParameter("@QuestionsInstructionsAudioFileId", listenPart.QuestionsInstructionsAudioFileFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}