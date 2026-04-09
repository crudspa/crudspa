namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolYearDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SchoolYear schoolYear)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolYearDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", schoolYear.Id);

        await command.Execute(connection, transaction);
    }
}