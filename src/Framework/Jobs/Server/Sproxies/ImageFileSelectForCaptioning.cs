namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class ImageFileSelectForCaptioning
{
    public static async Task<IList<ImageFile>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.ImageFileSelectForCaptioning";

        command.AddParameter("@SessionId", sessionId);

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
        };
    }
}