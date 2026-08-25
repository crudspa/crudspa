namespace Crudspa.Education.Student.Server.Sproxies;

public static class AssessmentAssignmentIsAccessible
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? id)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.AssessmentAssignmentIsAccessible";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);

        return await command.ExecuteScalarInt(connection) == 1;
    }
}