namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class AudioFileSelectForOptimization
{
    public static async Task<IList<AudioFile>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.AudioFileSelectForOptimization";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadAudioFile);
    }

    private static AudioFile ReadAudioFile(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BlobId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            Format = reader.ReadString(3),
            OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(4),
            OptimizedBlobId = reader.ReadGuid(5),
            OptimizedFormat = reader.ReadString(6),
        };
    }
}