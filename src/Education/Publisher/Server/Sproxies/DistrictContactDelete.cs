namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictContactDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, DistrictContact districtContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictContactDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", districtContact.Id);

        await command.Execute(connection, transaction);
    }
}