namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Campaign campaign)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.CampaignUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", campaign.Id);
        command.AddParameter("@Name", campaign.Name);
        command.AddParameter("@Description", campaign.Description);
        command.AddParameter("@Licenses", campaign.Licenses);
        await command.Execute(connection, transaction);
    }
}