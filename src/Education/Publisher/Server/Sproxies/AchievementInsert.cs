using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AchievementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Achievement achievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AchievementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Title", 75, achievement.Title);
        command.AddParameter("@RarityId", achievement.RarityId);
        command.AddParameter("@TrophyImageId", achievement.TrophyImageFile.Id);
        command.AddParameter("@VisibleToStudents", achievement.VisibleToStudents ?? true);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}