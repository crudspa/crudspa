namespace Crudspa.Framework.Core.Server.Sproxies;

public static class AudioFileUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AudioFile audioFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.AudioFileUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", audioFile.Id);
        command.AddParameter("@BlobId", audioFile.BlobId);
        command.AddParameter("@Name", 150, audioFile.Name?.Trim());
        command.AddParameter("@Format", 10, audioFile.Name.GetExtension());
        command.AddParameter("@OptimizedStatus", audioFile.OptimizedStatus);
        command.AddParameter("@OptimizedBlobId", audioFile.OptimizedBlobId);
        command.AddParameter("@OptimizedFormat", 10, audioFile.OptimizedFormat);

        await command.Execute(connection, transaction);
    }
}