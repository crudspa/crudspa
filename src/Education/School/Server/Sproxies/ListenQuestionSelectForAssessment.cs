namespace Crudspa.Education.School.Server.Sproxies;

public static class ListenQuestionSelectForAssessment
{
    public static async Task<IList<ListenQuestion>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ListenQuestionSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadListenQuestion);
    }

    private static ListenQuestion ReadListenQuestion(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ListenPartId = reader.ReadGuid(1),
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