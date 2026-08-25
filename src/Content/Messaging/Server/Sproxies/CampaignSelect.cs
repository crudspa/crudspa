namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignSelect
{
    public static async Task<Campaign?> Execute(String connection, Guid? sessionId, Campaign campaign)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.CampaignSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", campaign.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            campaign = ReadCampaign(reader);

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                campaign.Licenses.Add(reader.ReadSelectable());

            return campaign;
        });
    }

    private static Campaign ReadCampaign(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            Description = reader.ReadString(3),
        };
    }
}