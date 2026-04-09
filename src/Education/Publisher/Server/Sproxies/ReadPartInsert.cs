namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadPartInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadPart readPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadPartInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", readPart.AssessmentId);
        command.AddParameter("@Title", 100, readPart.Title);
        command.AddParameter("@PassageInstructionsText", readPart.PassageInstructionsText);
        command.AddParameter("@PassageInstructionsAudioFileId", readPart.PassageInstructionsAudioFileFile.Id);
        command.AddParameter("@PreviewInstructionsText", readPart.PreviewInstructionsText);
        command.AddParameter("@PreviewInstructionsAudioFileId", readPart.PreviewInstructionsAudioFileFile.Id);
        command.AddParameter("@QuestionsInstructionsText", readPart.QuestionsInstructionsText);
        command.AddParameter("@QuestionsInstructionsAudioFileId", readPart.QuestionsInstructionsAudioFileFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}