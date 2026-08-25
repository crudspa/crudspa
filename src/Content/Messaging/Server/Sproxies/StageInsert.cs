namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class StageInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Stage stage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.StageInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CampaignId", stage.CampaignId);
        command.AddParameter("@Name", stage.Name);
        command.AddParameter("@Offset", stage.Offset);
        command.AddParameter("@Anchor", stage.Anchor);
        command.AddParameter("@WeekendAdjustment", stage.WeekendAdjustment);
        command.AddParameter("@SendTime", stage.SendTime?.ToTimeSpan());

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}