namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobScheduleUpdateNextRun
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, JobSchedule jobSchedule)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobScheduleUpdateNextRun";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", jobSchedule.Id);
        command.AddParameter("@NextRun", jobSchedule.NextRun);

        await command.Execute(connection, transaction);
    }
}