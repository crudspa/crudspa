namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class StageSelect
{
    public static async Task<Stage?> Execute(String connection, Guid? sessionId, Stage stage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.StageSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", stage.Id);

        return await command.ReadSingle(connection, ReadStage);
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