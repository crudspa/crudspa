namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class AudioFileSelect
{
    public static async Task<AudioFile?> Execute(String connection, AudioFile audioFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.AudioFileSelect";

        command.AddParameter("@Id", audioFile.Id);

        return await command.ReadSingle(connection, ReadAudioFile);
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