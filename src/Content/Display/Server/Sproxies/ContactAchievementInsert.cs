namespace Crudspa.Content.Display.Server.Sproxies;

public static class ContactAchievementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactAchievement contactAchievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ContactAchievementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", contactAchievement.ContactId);
        command.AddParameter("@AchievementId", contactAchievement.Achievement.Id);
        command.AddParameter("@RelatedEntityId", contactAchievement.RelatedEntityId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}