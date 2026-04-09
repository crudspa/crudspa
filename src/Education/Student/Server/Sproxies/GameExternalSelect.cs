namespace Crudspa.Education.Student.Server.Sproxies;

public static class GameExternalSelect
{
    public static async Task<Game?> Execute(String connection, Game game, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.GameExternalSelect";

        command.AddParameter("@Id", game.Id);
        command.AddParameter("@SessionId", sessionId);

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
            BookTitle = reader.ReadString(5),
        };
    }
}