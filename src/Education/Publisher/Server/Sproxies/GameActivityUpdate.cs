namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameActivityUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GameActivity gameActivity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameActivityUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", gameActivity.Id);
        command.AddParameter("@ThemeWord", 50, gameActivity.ThemeWord);
        command.AddParameter("@GroupId", gameActivity.GroupId);
        command.AddParameter("@TypeId", gameActivity.TypeId);
        command.AddParameter("@Rigorous", gameActivity.Rigorous ?? false);
        command.AddParameter("@Multisyllabic", gameActivity.Multisyllabic ?? false);
        command.AddParameter("@ActivityId", gameActivity.ActivityId);

        await command.Execute(connection, transaction);
    }
}