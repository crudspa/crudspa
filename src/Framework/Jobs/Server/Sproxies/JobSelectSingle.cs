namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobSelectSingle
{
    public static async Task<Job?> Execute(String connection, Guid? sessionId, Guid? deviceId, Guid? jobId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobSelectSingle";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DeviceId", deviceId);
        command.AddParameter("@JobId", jobId);

        return await command.ReadSingle(connection, ReadJob);
    }

    public static Job ReadJob(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            TypeId = reader.ReadGuid(1),
            Config = reader.ReadString(2),
            Added = reader.ReadDateTimeOffset(3),
            Started = reader.ReadDateTimeOffset(4),
            Ended = reader.ReadDateTimeOffset(5),
            StatusId = reader.ReadGuid(6),
            DeviceId = reader.ReadGuid(7),
            ScheduleId = reader.ReadGuid(8),
            BatchId = reader.ReadGuid(9),
            Type = new()
            {
                Id = reader.ReadGuid(10),
                Name = reader.ReadString(11),
                EditorView = reader.ReadString(12),
                ActionClass = reader.ReadString(13),
            },
        };
    }
}