namespace Crudspa.Content.Design.Server.Sproxies;

public static class AchievementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Achievement achievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AchievementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", achievement.PortalId);
        command.AddParameter("@Title", 75, achievement.Title);
        command.AddParameter("@Description", achievement.Description);
        command.AddParameter("@ImageId", achievement.ImageFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}