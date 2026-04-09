namespace Crudspa.Education.School.Server.Sproxies;

public static class AssessmentAssignmentUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AssessmentAssignment assessmentAssignment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.AssessmentAssignmentUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessmentAssignment.Id);
        command.AddParameter("@StudentId", assessmentAssignment.StudentId);
        command.AddParameter("@AssessmentId", assessmentAssignment.AssessmentId);
        command.AddParameter("@StartAfter", assessmentAssignment.StartAfter);
        command.AddParameter("@EndBefore", assessmentAssignment.EndBefore);

        await command.Execute(connection, transaction);
    }
}