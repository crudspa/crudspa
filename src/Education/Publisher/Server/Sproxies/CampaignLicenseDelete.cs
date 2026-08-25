namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CampaignLicenseDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CampaignLicense campaignLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CampaignLicenseDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", campaignLicense.Id);

        await command.Execute(connection, transaction);
    }
}