namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobScheduleInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, JobSchedule jobSchedule)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobScheduleInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Name", 50, jobSchedule.Name);
        command.AddParameter("@TypeId", jobSchedule.TypeId);
        command.AddParameter("@Config", jobSchedule.Config);
        command.AddParameter("@Description", jobSchedule.Description);
        command.AddParameter("@DeviceId", jobSchedule.DeviceId);
        command.AddParameter("@RecurrenceAmount", jobSchedule.RecurrenceAmount);
        command.AddParameter("@RecurrenceInterval", jobSchedule.RecurrenceInterval);
        command.AddParameter("@RecurrencePattern", jobSchedule.RecurrencePattern);
        command.AddParameter("@Day", jobSchedule.Day);
        command.AddParameter("@DayOfWeek", jobSchedule.DayOfWeek);
        command.AddParameter("@Hour", jobSchedule.Hour);
        command.AddParameter("@Minute", jobSchedule.Minute);
        command.AddParameter("@Second", jobSchedule.Second);
        command.AddParameter("@TimeZoneId", 32, jobSchedule.TimeZoneId);
        command.AddParameter("@NextRun", jobSchedule.NextRun);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}