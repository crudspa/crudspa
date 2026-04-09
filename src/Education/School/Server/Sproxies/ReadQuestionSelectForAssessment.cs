namespace Crudspa.Education.School.Server.Sproxies;

public static class ReadQuestionSelectForAssessment
{
    public static async Task<IList<ReadQuestion>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ReadQuestionSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadReadQuestion);
    }

    private static ReadQuestion ReadReadQuestion(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ReadPartId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            IsPreview = reader.ReadBoolean(3),
            PageBreakBefore = reader.ReadBoolean(4),
            HasCorrectChoice = reader.ReadBoolean(5),
            RequireTextInput = reader.ReadBoolean(6),
            Ordinal = reader.ReadInt32(7),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(12),
                OptimizedBlobId = reader.ReadGuid(13),
                OptimizedFormat = reader.ReadString(14),
            },
        };
    }
}