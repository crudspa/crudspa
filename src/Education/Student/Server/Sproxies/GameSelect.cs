namespace Crudspa.Education.Student.Server.Sproxies;

public static class GameSelect
{
    public static async Task<Game?> Execute(String connection, Game game)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.GameSelect";

        command.AddParameter("@Id", game.Id);

        return await command.ReadSingle(connection, ReadGame);
    }

    private static Game ReadGame(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            Key = reader.ReadString(2),
            Title = reader.ReadString(3),
            IconName = reader.ReadString(4),
            StatusId = reader.ReadGuid(5),
            GradeId = reader.ReadGuid(6),
            AssessmentLevelId = reader.ReadGuid(7),
            RequiresAchievementId = reader.ReadGuid(8),
            GeneratesAchievementId = reader.ReadGuid(9),
        };
    }
}