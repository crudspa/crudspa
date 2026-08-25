namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Campaign campaign)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.CampaignDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", campaign.Id);

        await command.Execute(connection, transaction);
    }
}