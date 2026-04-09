namespace Crudspa.Content.Design.Server.Sproxies;

public static class VideoUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VideoElement video)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.VideoUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", video.Id);
        command.AddParameter("@FileId", video.FileFile.Id);
        command.AddParameter("@AutoPlay", video.AutoPlay ?? false);
        command.AddParameter("@PosterId", video.Poster.Id);

        await command.Execute(connection, transaction);
    }
}