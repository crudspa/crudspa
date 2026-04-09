namespace Crudspa.Education.School.Server.Sproxies;

public static class VocabChoiceSelectForAssessment
{
    public static async Task<IList<VocabChoice>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.VocabChoiceSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadVocabChoice);
    }

    private static VocabChoice ReadVocabChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            VocabQuestionId = reader.ReadGuid(1),
            Word = reader.ReadString(2),
            IsCorrect = reader.ReadBoolean(3),
            Ordinal = reader.ReadInt32(4),
        };
    }
}