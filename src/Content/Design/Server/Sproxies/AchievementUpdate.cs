namespace Crudspa.Content.Design.Server.Sproxies;

public static class AchievementUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Achievement achievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AchievementUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", achievement.Id);
        command.AddParameter("@Title", 75, achievement.Title);
        command.AddParameter("@Description", achievement.Description);
        command.AddParameter("@ImageId", achievement.ImageFile.Id);

        await command.Execute(connection, transaction);
    }
}