namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomTeacherUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassroomTeacher classroomTeacher)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomTeacherUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", classroomTeacher.Id);
        command.AddParameter("@ClassroomId", classroomTeacher.ClassroomId);
        command.AddParameter("@SchoolContactId", classroomTeacher.SchoolContactId);

        await command.Execute(connection, transaction);
    }
}