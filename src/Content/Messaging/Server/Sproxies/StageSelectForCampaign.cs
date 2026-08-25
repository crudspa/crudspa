namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class StageSelectForCampaign
{
    public static async Task<IList<Stage>> Execute(String connection, Guid? sessionId, Guid? campaignId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.StageSelectForCampaign";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CampaignId", campaignId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var stages = new List<Stage>();

            while (await reader.ReadAsync())
                stages.Add(ReadStage(reader));

            return stages;
        });
    }

    private static Stage ReadStage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            CampaignId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            Offset = reader.ReadInt32(3),
            Anchor = reader.ReadEnum<Stage.Anchors>(4),
            WeekendAdjustment = reader.ReadEnum<Stage.WeekendAdjustments>(5),
            SendTime = reader.ReadTimeOnly(6),
            Ordinal = reader.ReadInt32(7),
            MessageCount = reader.ReadInt32(8),
        };
    }
}