namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomStudentInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassroomStudent classroomStudent)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomStudentInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ClassroomId", classroomStudent.ClassroomId);
        command.AddParameter("@StudentId", classroomStudent.StudentId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}