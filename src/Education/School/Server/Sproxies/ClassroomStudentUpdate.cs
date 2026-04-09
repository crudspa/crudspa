namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomStudentUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassroomStudent classroomStudent)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomStudentUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", classroomStudent.Id);
        command.AddParameter("@ClassroomId", classroomStudent.ClassroomId);
        command.AddParameter("@StudentId", classroomStudent.StudentId);

        await command.Execute(connection, transaction);
    }
}