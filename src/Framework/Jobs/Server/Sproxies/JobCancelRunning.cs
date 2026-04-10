namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobCancelRunning
{
    public static async Task<IList<JobStatusChanged>?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? deviceId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobCancelRunning";

        command.AddParameter("@DeviceId", deviceId);

        return await command.ReadAll(connection, transaction, ReadJobStatusChanged);
    }

    private static JobStatusChanged ReadJobStatusChanged(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Started = reader.ReadDateTimeOffset(1),
            Ended = reader.ReadDateTimeOffset(2),
            StatusId = reader.ReadGuid(3),
            DeviceId = reader.ReadGuid(4),
        };
    }
}