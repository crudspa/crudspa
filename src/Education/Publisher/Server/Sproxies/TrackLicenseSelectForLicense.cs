namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TrackLicenseSelectForLicense
{
    public static async Task<IList<TrackLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TrackLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var trackLicenses = new List<TrackLicense>();

            while (await reader.ReadAsync())
                trackLicenses.Add(ReadTrackLicense(reader));

            return trackLicenses;
        });
    }

    private static TrackLicense ReadTrackLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            TrackId = reader.ReadGuid(2),
            TrackTitle = reader.ReadString(3),
        };
    }
}