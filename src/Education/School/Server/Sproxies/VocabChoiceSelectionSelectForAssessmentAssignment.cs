namespace Crudspa.Education.School.Server.Sproxies;

public static class VocabChoiceSelectionSelectForAssessmentAssignment
{
    public static async Task<IList<VocabChoiceSelection>> Execute(String connection, Guid? assessmentAssignmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.VocabChoiceSelectionSelectForAssessmentAssignment";

        command.AddParameter("@AssessmentAssignmentId", assessmentAssignmentId);

        return await command.ReadAll(connection, ReadVocabChoiceSelection);
    }

    private static VocabChoiceSelection ReadVocabChoiceSelection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ChoiceId = reader.ReadGuid(2),
            Made = reader.ReadDateTimeOffset(3),
            ChoiceVocabQuestionId = reader.ReadGuid(4),
            ChoiceText = reader.ReadString(5),
        };
    }
}