namespace Crudspa.Content.Design.Server.Sproxies;

public static class TrackUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Track track)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TrackUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", track.Id);
        command.AddParameter("@Title", 75, track.Title);
        command.AddParameter("@StatusId", track.StatusId);
        command.AddParameter("@Description", track.Description);
        command.AddParameter("@RequiresAchievementId", track.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", track.GeneratesAchievementId);
        command.AddParameter("@RequireSequentialCompletion", track.RequireSequentialCompletion ?? true);

        await command.Execute(connection, transaction);
    }
}