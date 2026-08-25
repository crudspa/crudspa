namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CampaignLicenseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CampaignLicense campaignLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CampaignLicenseInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseId", campaignLicense.LicenseId);
        command.AddParameter("@CampaignId", campaignLicense.CampaignId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}