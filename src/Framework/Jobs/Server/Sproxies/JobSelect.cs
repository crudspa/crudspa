namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobSelect
{
    public static async Task<Job?> Execute(String connection, Job job)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobSelect";

        command.AddParameter("@Id", job.Id);

        return await command.ReadSingle(connection, ReadJob);
    }

    private static Job ReadJob(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            TypeId = reader.ReadGuid(1),
            Config = reader.ReadString(2),
            Description = reader.ReadString(3),
            Added = reader.ReadDateTimeOffset(4),
            Started = reader.ReadDateTimeOffset(5),
            Ended = reader.ReadDateTimeOffset(6),
            StatusId = reader.ReadGuid(7),
            DeviceId = reader.ReadGuid(8),
            ScheduleId = reader.ReadGuid(9),
            BatchId = reader.ReadGuid(10),
            TypeName = reader.ReadString(11),
            StatusName = reader.ReadString(12),
            StatusCssClass = reader.ReadString(13),
            DeviceName = reader.ReadString(14),
            ScheduleName = reader.ReadString(15),
        };
    }
}