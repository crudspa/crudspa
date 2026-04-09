namespace Crudspa.Education.School.Server.Sproxies;

public static class ReadChoiceSelectionSelectForAssessmentAssignment
{
    public static async Task<IList<ReadChoiceSelection>> Execute(String connection, Guid? assessmentAssignmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ReadChoiceSelectionSelectForAssessmentAssignment";

        command.AddParameter("@AssessmentAssignmentId", assessmentAssignmentId);

        return await command.ReadAll(connection, ReadReadChoiceSelection);
    }

    private static ReadChoiceSelection ReadReadChoiceSelection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ChoiceId = reader.ReadGuid(2),
            Made = reader.ReadDateTimeOffset(3),
            ChoiceReadQuestionId = reader.ReadGuid(4),
            ChoiceText = reader.ReadString(5),
        };
    }
}