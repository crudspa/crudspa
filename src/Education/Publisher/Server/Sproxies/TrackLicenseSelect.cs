namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TrackLicenseSelect
{
    public static async Task<TrackLicense?> Execute(String connection, Guid? sessionId, TrackLicense trackLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TrackLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", trackLicense.Id);

        return await command.ReadSingle(connection, ReadTrackLicense);
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