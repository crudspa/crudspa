namespace Crudspa.Education.School.Server.Sproxies;

using School = Shared.Contracts.Data.School;

public static class SchoolUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, School school)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.SchoolUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", school.Id);

        await command.Execute(connection, transaction);
    }
}