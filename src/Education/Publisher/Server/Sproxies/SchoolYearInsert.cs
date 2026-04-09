namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolYearInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SchoolYear schoolYear)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolYearInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Name", 25, schoolYear.Name);
        command.AddParameter("@Starts", schoolYear.Starts);
        command.AddParameter("@Ends", schoolYear.Ends);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}