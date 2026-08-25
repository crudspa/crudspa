namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Campaign campaign)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.CampaignInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", campaign.PortalId);
        command.AddParameter("@Name", campaign.Name);
        command.AddParameter("@Description", campaign.Description);
        command.AddParameter("@Licenses", campaign.Licenses);
        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}