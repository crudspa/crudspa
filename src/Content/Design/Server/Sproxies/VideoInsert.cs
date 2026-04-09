namespace Crudspa.Content.Design.Server.Sproxies;

public static class VideoInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VideoElement video)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.VideoInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", video.ElementId);
        command.AddParameter("@FileId", video.FileFile.Id);
        command.AddParameter("@AutoPlay", video.AutoPlay ?? false);
        command.AddParameter("@PosterId", video.Poster.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}