namespace Crudspa.Education.Student.Server.Sproxies;

public static class GameProgressSelect
{
    public static async Task<GameProgress> Execute(String connection, Guid? sessionId, Guid? gameId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.GameProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@GameId", gameId);

        var progress = await command.ReadSingle(connection, ReadGameProgress);

        return progress ?? new()
        {
            GameId = gameId,
            GameCompletedCount = 0,
        };
    }

    public static GameProgress ReadGameProgress(SqlDataReader reader)
    {
        return new()
        {
            StudentId = reader.ReadGuid(0),
            GameId = reader.ReadGuid(1),
            BookId = reader.ReadGuid(2),
            GameCompletedCount = reader.ReadInt32(3),
        };
    }
}