namespace Crudspa.Education.School.Server.Sproxies;

public static class StudentSecretCodeIsAvailable
{
    public static async Task<Boolean> Execute(String connection, Guid? studentId, String secretCode)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.StudentSecretCodeIsAvailable";

        command.AddParameter("@StudentId", studentId);
        command.AddParameter("@SecretCode", 75, secretCode);

        return await command.ExecuteBoolean(connection, "@Available");
    }
}