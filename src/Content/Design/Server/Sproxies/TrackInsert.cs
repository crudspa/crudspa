namespace Crudspa.Content.Design.Server.Sproxies;

public static class TrackInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Track track)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TrackInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", track.PortalId);
        command.AddParameter("@Title", 75, track.Title);
        command.AddParameter("@StatusId", track.StatusId);
        command.AddParameter("@Description", track.Description);
        command.AddParameter("@RequiresAchievementId", track.RequiresAchievementId);
        command.AddParameter("@GeneratesAchievementId", track.GeneratesAchievementId);
        command.AddParameter("@RequireSequentialCompletion", track.RequireSequentialCompletion ?? true);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}