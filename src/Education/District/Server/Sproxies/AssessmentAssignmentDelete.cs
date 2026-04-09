namespace Crudspa.Education.District.Server.Sproxies;

public static class AssessmentAssignmentDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AssessmentAssignment assessmentAssignment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.AssessmentAssignmentDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessmentAssignment.Id);

        await command.Execute(connection, transaction);
    }
}