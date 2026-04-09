namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AchievementSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AchievementSelectNames";

        return await command.ReadNameds(connection);
    }
}