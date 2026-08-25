namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CampaignLicenseUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CampaignLicense campaignLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CampaignLicenseUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", campaignLicense.Id);
        command.AddParameter("@CampaignId", campaignLicense.CampaignId);

        await command.Execute(connection, transaction);
    }
}