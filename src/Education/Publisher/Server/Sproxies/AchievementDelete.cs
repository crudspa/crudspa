using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AchievementDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Achievement achievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AchievementDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", achievement.Id);

        await command.Execute(connection, transaction);
    }
}