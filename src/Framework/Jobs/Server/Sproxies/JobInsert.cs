namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Job job)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TypeId", job.TypeId);
        command.AddParameter("@Config", job.Config);
        command.AddParameter("@Description", job.Description);
        command.AddParameter("@DeviceId", job.DeviceId);
        command.AddParameter("@ScheduleId", job.ScheduleId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}