namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignSelectForPortal
{
    public static async Task<IList<Campaign>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.CampaignSelectForPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var campaigns = new List<Campaign>();

            while (await reader.ReadAsync())
                campaigns.Add(ReadCampaign(reader));

            return campaigns;
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