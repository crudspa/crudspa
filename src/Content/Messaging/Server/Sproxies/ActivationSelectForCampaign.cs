namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class ActivationSelectForCampaign
{
    public static async Task<IList<Activation>> Execute(String connection, Guid? sessionId, Guid? campaignId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.ActivationSelectForCampaign";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CampaignId", campaignId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var activations = new List<Activation>();

            while (await reader.ReadAsync())
                activations.Add(ReadActivation(reader));

            return activations;
        });
    }

    private static Activation ReadActivation(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
            OrganizationName = reader.ReadString(2),
            CampaignId = reader.ReadGuid(3),
            CampaignName = reader.ReadString(4),
            BatchId = reader.ReadGuid(5),
            Start = reader.ReadDateOnly(6),
            Activated = reader.ReadDateTimeOffset(7),
            ActivatedBy = reader.ReadGuid(8),
        };
    }
}