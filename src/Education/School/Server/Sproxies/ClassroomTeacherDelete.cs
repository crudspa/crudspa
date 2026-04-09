namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomTeacherDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassroomTeacher classroomTeacher)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomTeacherDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", classroomTeacher.Id);

        await command.Execute(connection, transaction);
    }
}