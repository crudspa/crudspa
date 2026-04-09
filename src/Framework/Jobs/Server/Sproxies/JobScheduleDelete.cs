namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobScheduleDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, JobSchedule jobSchedule)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobScheduleDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", jobSchedule.Id);

        await command.Execute(connection, transaction);
    }
}