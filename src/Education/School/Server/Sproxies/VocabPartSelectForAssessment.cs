namespace Crudspa.Education.School.Server.Sproxies;

public static class VocabPartSelectForAssessment
{
    public static async Task<IList<VocabPart>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.VocabPartSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadVocabPart);
    }

    private static VocabPart ReadVocabPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            PreviewInstructionsText = reader.ReadString(3),
            QuestionsInstructionsText = reader.ReadString(4),
            Ordinal = reader.ReadInt32(5),
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(10),
                OptimizedBlobId = reader.ReadGuid(11),
                OptimizedFormat = reader.ReadString(12),
            },
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
            VocabQuestionCount = reader.ReadInt32(20),
        };
    }
}