namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SegmentLicenseSelectForLicense
{
    public static async Task<IList<SegmentLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SegmentLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var segmentLicenses = new List<SegmentLicense>();

            while (await reader.ReadAsync())
                segmentLicenses.Add(ReadSegmentLicense(reader));

            return segmentLicenses;
        });
    }

    private static SegmentLicense ReadSegmentLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            SegmentId = reader.ReadGuid(2),
            SegmentTitle = reader.ReadString(3),
        };
    }
}