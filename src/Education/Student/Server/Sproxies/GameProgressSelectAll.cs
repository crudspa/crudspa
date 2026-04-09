namespace Crudspa.Education.Student.Server.Sproxies;

public static class GameProgressSelectAll
{
    public static async Task<IList<GameProgress>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.GameProgressSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadGameProgress);
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