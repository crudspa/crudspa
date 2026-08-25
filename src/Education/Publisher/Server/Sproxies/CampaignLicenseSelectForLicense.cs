namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CampaignLicenseSelectForLicense
{
    public static async Task<IList<CampaignLicense>> Execute(String connection, Guid? sessionId, Guid? licenseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CampaignLicenseSelectForLicense";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", licenseId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var campaignLicenses = new List<CampaignLicense>();

            while (await reader.ReadAsync())
                campaignLicenses.Add(ReadCampaignLicense(reader));

            return campaignLicenses;
        });
    }

    private static CampaignLicense ReadCampaignLicense(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LicenseId = reader.ReadGuid(1),
            CampaignId = reader.ReadGuid(2),
            CampaignName = reader.ReadString(3),
        };
    }
}