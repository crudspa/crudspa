namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadPartSelectForAssessment
{
    public static async Task<IList<ReadPart>> Execute(String connection, Guid? sessionId, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadPartSelectForAssessment";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadReadPart);
    }

    private static ReadPart ReadReadPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            PassageInstructionsText = reader.ReadString(3),
            PassageInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(8),
                OptimizedBlobId = reader.ReadGuid(9),
                OptimizedFormat = reader.ReadString(10),
            },
            PreviewInstructionsText = reader.ReadString(11),
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(16),
                OptimizedBlobId = reader.ReadGuid(17),
                OptimizedFormat = reader.ReadString(18),
            },
            QuestionsInstructionsText = reader.ReadString(19),
            QuestionsInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(20),
                BlobId = reader.ReadGuid(21),
                Name = reader.ReadString(22),
                Format = reader.ReadString(23),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(24),
                OptimizedBlobId = reader.ReadGuid(25),
                OptimizedFormat = reader.ReadString(26),
            },
            Ordinal = reader.ReadInt32(27),
            ReadParagraphCount = reader.ReadInt32(28),
            ReadQuestionCount = reader.ReadInt32(29),
        };
    }
}