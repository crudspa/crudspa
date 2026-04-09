namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameSectionSelectForGame
{
    public static async Task<IList<GameSection>> Execute(String connection, Guid? sessionId, Guid? gameId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameSectionSelectForGame";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@GameId", gameId);

        return await command.ReadAll(connection, ReadGameSection);
    }

    private static GameSection ReadGameSection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            GameId = reader.ReadGuid(1),
            GameKey = reader.ReadString(2),
            Title = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            TypeId = reader.ReadGuid(6),
            TypeName = reader.ReadString(7),
            Ordinal = reader.ReadInt32(8),
            GameActivityCount = reader.ReadInt32(9),
        };
    }
}