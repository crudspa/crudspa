namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobStatusSelectOrderables
{
    public static async Task<IList<OrderableCssClass>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobStatusSelectOrderables";

        return await command.ReadOrderableCssClasses(connection);
    }
}