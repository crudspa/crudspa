namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobScheduleSelect
{
    public static async Task<JobSchedule?> Execute(String connection, JobSchedule jobSchedule)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobScheduleSelect";

        command.AddParameter("@Id", jobSchedule.Id);

        return await command.ReadSingle(connection, ReadJobSchedule);
    }

    public static JobSchedule ReadJobSchedule(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            TypeId = reader.ReadGuid(2),
            Config = reader.ReadString(3),
            Description = reader.ReadString(4),
            DeviceId = reader.ReadGuid(5),
            RecurrenceAmount = reader.ReadInt32(6),
            RecurrenceInterval = reader.ReadEnum<JobSchedule.RecurrenceIntervals>(7),
            RecurrencePattern = reader.ReadEnum<JobSchedule.RecurrencePatterns>(8),
            Day = reader.ReadInt32(9),
            DayOfWeek = reader.ReadEnum<JobSchedule.DayOfWeeks>(10),
            Hour = reader.ReadInt32(11),
            Minute = reader.ReadInt32(12),
            Second = reader.ReadInt32(13),
            TimeZoneId = reader.ReadString(14),
            NextRun = reader.ReadDateTimeOffset(15),
            TypeName = reader.ReadString(16),
            DeviceName = reader.ReadString(17),
        };
    }
}