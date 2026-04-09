namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class AudioFileUpdateOptimizationStatus
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AudioFile audioFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.AudioFileUpdateOptimizationStatus";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", audioFile.Id);
        command.AddParameter("@OptimizedStatus", audioFile.OptimizedStatus);
        command.AddParameter("@OptimizedBlobId", audioFile.OptimizedBlobId);
        command.AddParameter("@OptimizedFormat", 10, audioFile.OptimizedFormat);

        await command.Execute(connection, transaction);
    }
}