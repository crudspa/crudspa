namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobScheduleSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobScheduleSelectNames";

        return await command.ReadNameds(connection);
    }
}