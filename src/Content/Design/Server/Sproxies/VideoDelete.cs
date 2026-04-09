namespace Crudspa.Content.Design.Server.Sproxies;

public static class VideoDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VideoElement video)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.VideoDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", video.Id);

        await command.Execute(connection, transaction);
    }
}