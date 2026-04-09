namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobScheduleSelectOverdue
{
    public static async Task<IList<JobSchedule>?> Execute(String connection, Guid? sessionId, Guid? deviceId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobScheduleSelectOverdue";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DeviceId", deviceId);

        return await command.ReadAll(connection, JobScheduleSelect.ReadJobSchedule);
    }
}