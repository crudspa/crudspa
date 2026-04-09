namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Game game)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", game.Id);
        command.AddParameter("@Key", 75, game.Key);
        command.AddParameter("@Title", 75, game.Title);
        command.AddParameter("@StatusId", game.StatusId);
        command.AddParameter("@IconId", game.IconId);
        command.AddParameter("@GradeId", game.GradeId);
        command.AddParameter("@AssessmentLevelId", game.AssessmentLevelId);
        command.AddParameter("@RequiresAchievementId", game.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", game.GeneratesAchievementId);

        await command.Execute(connection, transaction);
    }
}