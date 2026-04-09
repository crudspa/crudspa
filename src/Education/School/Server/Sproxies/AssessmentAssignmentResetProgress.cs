namespace Crudspa.Education.School.Server.Sproxies;

public static class AssessmentAssignmentResetProgress
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AssessmentAssignment assessmentAssignment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.AssessmentAssignmentResetProgress";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessmentAssignment.Id);

        await command.Execute(connection, transaction);
    }
}