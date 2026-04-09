namespace Crudspa.Framework.Core.Server.Sproxies;

public static class AudioFileInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AudioFile audioFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.AudioFileInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BlobId", audioFile.BlobId);
        command.AddParameter("@Name", 150, audioFile.Name?.Trim());
        command.AddParameter("@Format", 10, audioFile.Name.GetExtension());
        command.AddParameter("@OptimizedStatus", audioFile.OptimizedStatus);
        command.AddParameter("@OptimizedBlobId", audioFile.OptimizedBlobId);
        command.AddParameter("@OptimizedFormat", 10, audioFile.OptimizedFormat);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}