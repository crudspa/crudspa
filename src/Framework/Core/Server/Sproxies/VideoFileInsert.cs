namespace Crudspa.Framework.Core.Server.Sproxies;

public static class VideoFileInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VideoFile videoFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.VideoFileInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BlobId", videoFile.BlobId);
        command.AddParameter("@Name", 150, videoFile.Name?.Trim());
        command.AddParameter("@Format", 10, videoFile.Name.GetExtension());
        command.AddParameter("@OptimizedStatus", videoFile.OptimizedStatus);
        command.AddParameter("@OptimizedBlobId", videoFile.OptimizedBlobId);
        command.AddParameter("@OptimizedFormat", 10, videoFile.OptimizedFormat);
        command.AddParameter("@PosterBlobId", videoFile.PosterBlobId);
        command.AddParameter("@PosterFormat", 10, videoFile.PosterFormat);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}