namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobSelectBatch
{
    public static async Task<IList<Job>?> Execute(String connection, Guid? sessionId, Guid? deviceId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobSelectBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DeviceId", deviceId);

        return await command.ReadAll(connection, JobSelectSingle.ReadJob);
    }
}