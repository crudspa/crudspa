namespace Crudspa.Content.Design.Server.Sproxies;

public static class TrackDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Track track)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TrackDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", track.Id);

        await command.Execute(connection, transaction);
    }
}