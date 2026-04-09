namespace Crudspa.Education.Student.Server.Sproxies;

public static class StudentUpdateAcceptTerms
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentUpdateAcceptTerms";

        command.AddParameter("@SessionId", sessionId);

        await command.Execute(connection, transaction);
    }
}