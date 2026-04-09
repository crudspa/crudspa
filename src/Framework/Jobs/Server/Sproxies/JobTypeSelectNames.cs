namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobTypeSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobTypeSelectNames";

        return await command.ReadNameds(connection);
    }
}