namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TrackLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, TrackLicense trackLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TrackLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", trackLicense.Id);

        await command.Execute(connection, transaction);
    }
}