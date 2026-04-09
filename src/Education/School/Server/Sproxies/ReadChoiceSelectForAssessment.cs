namespace Crudspa.Education.School.Server.Sproxies;

public static class ReadChoiceSelectForAssessment
{
    public static async Task<IList<ReadChoice>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ReadChoiceSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadReadChoice);
    }

    private static ReadChoice ReadReadChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ReadQuestionId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            IsCorrect = reader.ReadBoolean(3),
            Ordinal = reader.ReadInt32(4),
        };
    }
}