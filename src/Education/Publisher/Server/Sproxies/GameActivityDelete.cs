namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameActivityDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GameActivity gameActivity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameActivityDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", gameActivity.Id);

        await command.Execute(connection, transaction);
    }
}