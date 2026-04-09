namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Student student)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", student.Id);

        await command.Execute(connection, transaction);
    }
}