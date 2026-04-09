namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameSelect
{
    public static async Task<Game?> Execute(String connection, Guid? sessionId, Game game)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", game.Id);

        return await command.ReadSingle(connection, ReadGame);
    }

    private static Game ReadGame(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            BookKey = reader.ReadString(2),
            Key = reader.ReadString(3),
            Title = reader.ReadString(4),
            StatusId = reader.ReadGuid(5),
            StatusName = reader.ReadString(6),
            IconId = reader.ReadGuid(7),
            IconName = reader.ReadString(8),
            GradeId = reader.ReadGuid(9),
            GradeName = reader.ReadString(10),
            AssessmentLevelId = reader.ReadGuid(11),
            AssessmentLevelKey = reader.ReadString(12),
            RequiresAchievementId = reader.ReadGuid(13),
            RequiresAchievementTitle = reader.ReadString(14),
            GeneratesAchievementId = reader.ReadGuid(15),
            GeneratesAchievementTitle = reader.ReadString(16),
            GameSectionCount = reader.ReadInt32(17),
        };
    }
}