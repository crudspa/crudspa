using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AchievementUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Achievement achievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AchievementUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", achievement.Id);
        command.AddParameter("@Title", 75, achievement.Title);
        command.AddParameter("@RarityId", achievement.RarityId);
        command.AddParameter("@TrophyImageId", achievement.TrophyImageFile.Id);
        command.AddParameter("@VisibleToStudents", achievement.VisibleToStudents ?? true);

        await command.Execute(connection, transaction);
    }
}