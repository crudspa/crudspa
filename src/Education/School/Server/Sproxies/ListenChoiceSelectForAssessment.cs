namespace Crudspa.Education.School.Server.Sproxies;

public static class ListenChoiceSelectForAssessment
{
    public static async Task<IList<ListenChoice>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ListenChoiceSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadListenChoice);
    }

    private static ListenChoice ReadListenChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ListenQuestionId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            IsCorrect = reader.ReadBoolean(3),
            Ordinal = reader.ReadInt32(4),
        };
    }
}