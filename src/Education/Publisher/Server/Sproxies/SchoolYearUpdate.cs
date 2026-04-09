namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolYearUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SchoolYear schoolYear)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolYearUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", schoolYear.Id);
        command.AddParameter("@Name", 25, schoolYear.Name);
        command.AddParameter("@Starts", schoolYear.Starts);
        command.AddParameter("@Ends", schoolYear.Ends);

        await command.Execute(connection, transaction);
    }
}