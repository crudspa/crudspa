namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameSectionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GameSection gameSection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameSectionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@GameId", gameSection.GameId);
        command.AddParameter("@Title", 75, gameSection.Title);
        command.AddParameter("@StatusId", gameSection.StatusId);
        command.AddParameter("@TypeId", gameSection.TypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}