namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Game game)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", game.Id);

        await command.Execute(connection, transaction);
    }
}