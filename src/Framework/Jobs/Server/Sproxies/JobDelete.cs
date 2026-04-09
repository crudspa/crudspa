namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Job job)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", job.Id);

        await command.Execute(connection, transaction);
    }
}