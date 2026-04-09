namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, District district)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", district.Id);

        await command.Execute(connection, transaction);
    }
}