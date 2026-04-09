namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class ImageFileSelectBeenOptimized
{
    public static async Task<IList<ImageFile>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.ImageFileSelectBeenOptimized";

        return await command.ReadAll(connection, ReadImageFile);
    }

    public static ImageFile ReadImageFile(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BlobId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            Format = reader.ReadString(3),
            Width = reader.ReadInt32(4),
            Height = reader.ReadInt32(5),
            Caption = reader.ReadString(6),
            OptimizedStatus = reader.ReadEnum<ImageFile.OptimizationStatus>(7),
            OptimizedBlobId = reader.ReadGuid(8),
            OptimizedFormat = reader.ReadString(9),
            Resized96BlobId = reader.ReadGuid(10),
            Resized192BlobId = reader.ReadGuid(11),
            Resized360BlobId = reader.ReadGuid(12),
            Resized540BlobId = reader.ReadGuid(13),
            Resized720BlobId = reader.ReadGuid(14),
            Resized1080BlobId = reader.ReadGuid(15),
            Resized1440BlobId = reader.ReadGuid(16),
            Resized1920BlobId = reader.ReadGuid(17),
            Resized3840BlobId = reader.ReadGuid(18),
        };
    }
}