namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Classroom classroom)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", classroom.Id);

        await command.Execute(connection, transaction);
    }
}