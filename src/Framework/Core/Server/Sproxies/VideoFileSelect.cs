namespace Crudspa.Framework.Core.Server.Sproxies;

public static class VideoFileSelect
{
    public static async Task<VideoFile?> Execute(String connection, VideoFile videoFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.VideoFileSelect";

        command.AddParameter("@Id", videoFile.Id);

        return await command.ReadSingle(connection, ReadVideoFile);
    }

    private static VideoFile ReadVideoFile(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BlobId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            Format = reader.ReadString(3),
            Width = reader.ReadInt32(4),
            Height = reader.ReadInt32(5),
            OptimizedStatus = reader.ReadEnum<VideoFile.OptimizationStatus>(6),
            OptimizedBlobId = reader.ReadGuid(7),
            OptimizedFormat = reader.ReadString(8),
            PosterBlobId = reader.ReadGuid(9),
            PosterFormat = reader.ReadString(10),
        };
    }
}