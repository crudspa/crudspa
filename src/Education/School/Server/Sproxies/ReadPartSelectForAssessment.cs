namespace Crudspa.Education.School.Server.Sproxies;

public static class ReadPartSelectForAssessment
{
    public static async Task<IList<ReadPart>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ReadPartSelectForAssessment";

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
            PassageInstructionsAudioFileId = reader.ReadGuid(4),
            PreviewInstructionsText = reader.ReadString(5),
            PreviewInstructionsAudioFileId = reader.ReadGuid(6),
            QuestionsInstructionsText = reader.ReadString(7),
            QuestionsInstructionsAudioFileId = reader.ReadGuid(8),
            Ordinal = reader.ReadInt32(9),
            PassageInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(10),
                BlobId = reader.ReadGuid(11),
                Name = reader.ReadString(12),
                Format = reader.ReadString(13),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(14),
                OptimizedBlobId = reader.ReadGuid(15),
                OptimizedFormat = reader.ReadString(16),
            },
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(17),
                BlobId = reader.ReadGuid(18),
                Name = reader.ReadString(19),
                Format = reader.ReadString(20),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(21),
                OptimizedBlobId = reader.ReadGuid(22),
                OptimizedFormat = reader.ReadString(23),
            },
            QuestionsInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(24),
                BlobId = reader.ReadGuid(25),
                Name = reader.ReadString(26),
                Format = reader.ReadString(27),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(28),
                OptimizedBlobId = reader.ReadGuid(29),
                OptimizedFormat = reader.ReadString(30),
            },
            ReadParagraphCount = reader.ReadInt32(31),
            ReadQuestionCount = reader.ReadInt32(32),
        };
    }
}