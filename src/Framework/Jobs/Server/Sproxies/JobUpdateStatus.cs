namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobUpdateStatus
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Job job)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobUpdateStatus";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", job.Id);
        command.AddParameter("@Ended", job.Ended);
        command.AddParameter("@StatusId", job.StatusId);

        await command.Execute(connection, transaction);
    }
}