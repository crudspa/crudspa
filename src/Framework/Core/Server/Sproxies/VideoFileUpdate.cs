namespace Crudspa.Framework.Core.Server.Sproxies;

public static class VideoFileUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VideoFile videoFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.VideoFileUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", videoFile.Id);
        command.AddParameter("@BlobId", videoFile.BlobId);
        command.AddParameter("@Name", 150, videoFile.Name?.Trim());
        command.AddParameter("@Format", 10, videoFile.Name.GetExtension());
        command.AddParameter("@OptimizedStatus", videoFile.OptimizedStatus);
        command.AddParameter("@OptimizedBlobId", videoFile.OptimizedBlobId);
        command.AddParameter("@OptimizedFormat", 10, videoFile.OptimizedFormat);
        command.AddParameter("@PosterBlobId", videoFile.PosterBlobId);
        command.AddParameter("@PosterFormat", 10, videoFile.PosterFormat);

        await command.Execute(connection, transaction);
    }
}