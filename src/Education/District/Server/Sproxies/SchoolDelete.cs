namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, School school)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", school.Id);

        await command.Execute(connection, transaction);
    }
}