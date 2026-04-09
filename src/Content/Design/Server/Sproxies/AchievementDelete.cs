namespace Crudspa.Content.Design.Server.Sproxies;

public static class AchievementDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Achievement achievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AchievementDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", achievement.Id);

        await command.Execute(connection, transaction);
    }
}