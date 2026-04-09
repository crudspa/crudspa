namespace Crudspa.Education.Student.Server.Sproxies;

public static class AssessmentAssignmentMarkCompleted
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? assessmentAssignmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.AssessmentAssignmentMarkCompleted";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessmentAssignmentId);

        await command.Execute(connection, transaction);
    }
}