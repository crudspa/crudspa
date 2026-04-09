namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameSectionSelect
{
    public static async Task<GameSection?> Execute(String connection, Guid? sessionId, GameSection gameSection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameSectionSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", gameSection.Id);

        return await command.ReadSingle(connection, ReadGameSection);
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