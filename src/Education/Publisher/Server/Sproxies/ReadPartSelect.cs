namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadPartSelect
{
    public static async Task<ReadPart?> Execute(String connection, Guid? sessionId, ReadPart readPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadPartSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readPart.Id);

        return await command.ReadSingle(connection, ReadReadPart);
    }

    private static ReadPart ReadReadPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            AssessmentName = reader.ReadString(2),
            Title = reader.ReadString(3),
            PassageInstructionsText = reader.ReadString(4),
            PassageInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(9),
                OptimizedBlobId = reader.ReadGuid(10),
                OptimizedFormat = reader.ReadString(11),
            },
            PreviewInstructionsText = reader.ReadString(12),
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(13),
                BlobId = reader.ReadGuid(14),
                Name = reader.ReadString(15),
                Format = reader.ReadString(16),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(17),
                OptimizedBlobId = reader.ReadGuid(18),
                OptimizedFormat = reader.ReadString(19),
            },
            QuestionsInstructionsText = reader.ReadString(20),
            QuestionsInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(21),
                BlobId = reader.ReadGuid(22),
                Name = reader.ReadString(23),
                Format = reader.ReadString(24),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(25),
                OptimizedBlobId = reader.ReadGuid(26),
                OptimizedFormat = reader.ReadString(27),
            },
            Ordinal = reader.ReadInt32(28),
            ReadParagraphCount = reader.ReadInt32(29),
            ReadQuestionCount = reader.ReadInt32(30),
        };
    }
}