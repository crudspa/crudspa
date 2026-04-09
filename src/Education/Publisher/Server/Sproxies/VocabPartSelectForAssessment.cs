namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabPartSelectForAssessment
{
    public static async Task<IList<VocabPart>> Execute(String connection, Guid? sessionId, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabPartSelectForAssessment";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadVocabPart);
    }

    private static VocabPart ReadVocabPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            AssessmentName = reader.ReadString(2),
            Title = reader.ReadString(3),
            PreviewInstructionsText = reader.ReadString(4),
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(9),
                OptimizedBlobId = reader.ReadGuid(10),
                OptimizedFormat = reader.ReadString(11),
            },
            QuestionsInstructionsText = reader.ReadString(12),
            QuestionsInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(13),
                BlobId = reader.ReadGuid(14),
                Name = reader.ReadString(15),
                Format = reader.ReadString(16),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(17),
                OptimizedBlobId = reader.ReadGuid(18),
                OptimizedFormat = reader.ReadString(19),
            },
            Ordinal = reader.ReadInt32(20),
            VocabQuestionCount = reader.ReadInt32(21),
        };
    }
}