namespace Crudspa.Education.School.Server.Sproxies;

public static class AssessmentAssignmentInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AssessmentAssignment assessmentAssignment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.AssessmentAssignmentInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@StudentId", assessmentAssignment.StudentId);
        command.AddParameter("@AssessmentId", assessmentAssignment.AssessmentId);
        command.AddParameter("@StartAfter", assessmentAssignment.StartAfter);
        command.AddParameter("@EndBefore", assessmentAssignment.EndBefore);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}