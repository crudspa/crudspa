namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomTeacherInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassroomTeacher classroomTeacher)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomTeacherInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ClassroomId", classroomTeacher.ClassroomId);
        command.AddParameter("@SchoolContactId", classroomTeacher.SchoolContactId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}