namespace Crudspa.Education.Student.Server.Sproxies;

public static class StudentUpdateProfile
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Shared.Contracts.Data.Student student)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentUpdateProfile";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AvatarString", 2, student.AvatarString);
        command.AddParameter("@PreferredName", 75, student.PreferredName);

        await command.Execute(connection, transaction);
    }
}