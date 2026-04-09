namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class ImageFileUpdateOptimizationStatus
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ImageFile imageFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.ImageFileUpdateOptimizationStatus";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", imageFile.Id);
        command.AddParameter("@Width", imageFile.Width);
        command.AddParameter("@Height", imageFile.Height);
        command.AddParameter("@OptimizedStatus", imageFile.OptimizedStatus);
        command.AddParameter("@OptimizedBlobId", imageFile.OptimizedBlobId);
        command.AddParameter("@OptimizedFormat", 10, imageFile.OptimizedFormat);
        command.AddParameter("@Resized96BlobId", imageFile.Resized96BlobId);
        command.AddParameter("@Resized192BlobId", imageFile.Resized192BlobId);
        command.AddParameter("@Resized360BlobId", imageFile.Resized360BlobId);
        command.AddParameter("@Resized540BlobId", imageFile.Resized540BlobId);
        command.AddParameter("@Resized720BlobId", imageFile.Resized720BlobId);
        command.AddParameter("@Resized1080BlobId", imageFile.Resized1080BlobId);
        command.AddParameter("@Resized1440BlobId", imageFile.Resized1440BlobId);
        command.AddParameter("@Resized1920BlobId", imageFile.Resized1920BlobId);
        command.AddParameter("@Resized3840BlobId", imageFile.Resized3840BlobId);

        await command.Execute(connection, transaction);
    }
}