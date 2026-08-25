namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TrackLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, TrackLicense trackLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TrackLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", trackLicense.LicenseId);
        command.AddParameter("@TrackId", trackLicense.TrackId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}