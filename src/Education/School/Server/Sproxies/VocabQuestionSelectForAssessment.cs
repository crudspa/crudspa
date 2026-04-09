namespace Crudspa.Education.School.Server.Sproxies;

public static class VocabQuestionSelectForAssessment
{
    public static async Task<IList<VocabQuestion>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.VocabQuestionSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadVocabQuestion);
    }

    private static VocabQuestion ReadVocabQuestion(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            VocabPartId = reader.ReadGuid(1),
            Word = reader.ReadString(2),
            IsPreview = reader.ReadBoolean(3),
            PageBreakBefore = reader.ReadBoolean(4),
            Ordinal = reader.ReadInt32(5),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(10),
                OptimizedBlobId = reader.ReadGuid(11),
                OptimizedFormat = reader.ReadString(12),
            },
        };
    }
}