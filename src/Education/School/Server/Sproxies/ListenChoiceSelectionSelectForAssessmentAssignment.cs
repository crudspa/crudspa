namespace Crudspa.Education.School.Server.Sproxies;

public static class ListenChoiceSelectionSelectForAssessmentAssignment
{
    public static async Task<IList<ListenChoiceSelection>> Execute(String connection, Guid? assessmentAssignmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ListenChoiceSelectionSelectForAssessmentAssignment";

        command.AddParameter("@AssessmentAssignmentId", assessmentAssignmentId);

        return await command.ReadAll(connection, ReadListenChoiceSelection);
    }

    private static ListenChoiceSelection ReadListenChoiceSelection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ChoiceId = reader.ReadGuid(2),
            Made = reader.ReadDateTimeOffset(3),
            ChoiceListenQuestionId = reader.ReadGuid(4),
            ChoiceText = reader.ReadString(5),
        };
    }
}