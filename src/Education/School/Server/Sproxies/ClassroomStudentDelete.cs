namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomStudentDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassroomStudent classroomStudent)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomStudentDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", classroomStudent.Id);

        await command.Execute(connection, transaction);
    }
}