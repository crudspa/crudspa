namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameActivityShareWithSection
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GameActivityShare gameActivityShare)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameActivityShareWithSection";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SourceGameActivityId", gameActivityShare.SourceGameActivityId);
        command.AddParameter("@TargetGameSectionId", gameActivityShare.TargetGameSectionId);

        await command.Execute(connection, transaction);
    }
}