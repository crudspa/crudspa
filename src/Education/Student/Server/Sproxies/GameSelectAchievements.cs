namespace Crudspa.Education.Student.Server.Sproxies;

public static class GameSelectAchievements
{
    public static async Task<Game?> Execute(String connection, Guid? gameId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.GameSelectAchievements";

        command.AddParameter("@GameId", gameId);

        return await command.ReadSingle(connection, ReadGame);
    }

    private static Game ReadGame(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            GeneratesAchievement = new()
            {
                Id = reader.ReadGuid(2),
                Title = reader.ReadString(3),
                RarityId = reader.ReadGuid(4),
                TrophyImageId = reader.ReadGuid(5),
                VisibleToStudents = reader.ReadBoolean(6),
            },
            RequiresAchievement = new()
            {
                Id = reader.ReadGuid(7),
                Title = reader.ReadString(8),
                RarityId = reader.ReadGuid(9),
                TrophyImageId = reader.ReadGuid(10),
                VisibleToStudents = reader.ReadBoolean(11),
            },
        };
    }
}