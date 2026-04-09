namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, School school)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", school.Id);

        await command.Execute(connection, transaction);
    }
}