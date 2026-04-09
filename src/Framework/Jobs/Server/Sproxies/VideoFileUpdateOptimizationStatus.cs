namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class VideoFileUpdateOptimizationStatus
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VideoFile videoFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.VideoFileUpdateOptimizationStatus";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", videoFile.Id);
        command.AddParameter("@Width", videoFile.Width);
        command.AddParameter("@Height", videoFile.Height);
        command.AddParameter("@OptimizedStatus", videoFile.OptimizedStatus);
        command.AddParameter("@OptimizedBlobId", videoFile.OptimizedBlobId);
        command.AddParameter("@OptimizedFormat", 10, videoFile.OptimizedFormat);
        command.AddParameter("@PosterBlobId", videoFile.PosterBlobId);
        command.AddParameter("@PosterFormat", 10, videoFile.PosterFormat);

        await command.Execute(connection, transaction);
    }
}