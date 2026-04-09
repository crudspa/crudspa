namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobScheduleCreateJob
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, JobSchedule jobSchedule)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobScheduleCreateJob";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", jobSchedule.Id);
        command.AddParameter("@NextRun", jobSchedule.NextRun);
        var output = command.AddOutputParameter("@JobId");

        await command.Execute(connection, transaction);

        return (Guid?)output.Value;
    }
}