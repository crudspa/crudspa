namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobCancelRunning
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? deviceId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobCancelRunning";

        command.AddParameter("@DeviceId", deviceId);

        await command.Execute(connection, transaction);
    }
}