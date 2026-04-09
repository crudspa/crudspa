namespace Crudspa.Content.Display.Server.Sproxies;

public static class ContactAchievementSelectUnlocks
{
    public static async Task<ContactUnlocks?> Execute(String connection, ContactAchievement contactAchievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ContactAchievementSelectUnlocks";

        command.AddParameter("@ContactId", contactAchievement.ContactId);
        command.AddParameter("@AchievementId", contactAchievement.Achievement.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var unlocks = new ContactUnlocks();

            while (await reader.ReadAsync())
                unlocks.Courses.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    Description = reader.ReadString(2),
                    TrackId = reader.ReadGuid(3),
                    TrackTitle = reader.ReadString(4),
                    TrackDescription = reader.ReadString(5),
                });

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unlocks.Tracks.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    Description = reader.ReadString(2),
                });

            return unlocks;
        });
    }
}