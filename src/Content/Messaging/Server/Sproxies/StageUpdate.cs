namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class StageUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Stage stage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.StageUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", stage.Id);
        command.AddParameter("@Name", stage.Name);
        command.AddParameter("@Offset", stage.Offset);
        command.AddParameter("@Anchor", stage.Anchor);
        command.AddParameter("@WeekendAdjustment", stage.WeekendAdjustment);
        command.AddParameter("@SendTime", stage.SendTime?.ToTimeSpan());

        await command.Execute(connection, transaction);
    }
}