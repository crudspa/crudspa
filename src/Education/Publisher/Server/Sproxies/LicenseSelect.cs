namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LicenseSelect
{
    public static async Task<License?> Execute(String connection, Guid? sessionId, License license)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", license.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            license = ReadLicense(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                license.Segments.Add(reader.ReadSelectable());

            return license;
        });
    }

    private static License ReadLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            Description = reader.ReadString(2),
            DistrictLicenseCount = reader.ReadInt32(3),
            UnitLicenseCount = reader.ReadInt32(4),
            SegmentLicenseCount = reader.ReadInt32(5),
            AssessmentLicenseCount = reader.ReadInt32(6),
            BlogLicenseCount = reader.ReadInt32(7),
            ForumLicenseCount = reader.ReadInt32(8),
            CampaignLicenseCount = reader.ReadInt32(9),
            TrackLicenseCount = reader.ReadInt32(10),
            SurveyLicenseCount = reader.ReadInt32(11),
        };
    }
}