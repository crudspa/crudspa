namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Game game)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", game.BookId);
        command.AddParameter("@Key", 75, game.Key);
        command.AddParameter("@Title", 75, game.Title);
        command.AddParameter("@StatusId", game.StatusId);
        command.AddParameter("@IconId", game.IconId);
        command.AddParameter("@GradeId", game.GradeId);
        command.AddParameter("@AssessmentLevelId", game.AssessmentLevelId);
        command.AddParameter("@RequiresAchievementId", game.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", game.GeneratesAchievementId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}