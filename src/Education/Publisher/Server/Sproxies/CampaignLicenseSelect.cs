namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CampaignLicenseSelect
{
    public static async Task<CampaignLicense?> Execute(String connection, Guid? sessionId, CampaignLicense campaignLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CampaignLicenseSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", campaignLicense.Id);

        return await command.ReadSingle(connection, ReadCampaignLicense);
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