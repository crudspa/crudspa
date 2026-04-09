namespace Crudspa.Content.Display.Server.Sproxies;

public static class ContactAchievementExists
{
    public static async Task<Boolean> Execute(String connection, ContactAchievement contactAchievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ContactAchievementExists";

        command.AddParameter("@ContactId", contactAchievement.ContactId);
        command.AddParameter("@AchievementId", contactAchievement.Achievement.Id);

        return await command.ExecuteBoolean(connection, "@AlreadyUnlocked");
    }
}