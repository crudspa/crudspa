namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class DeviceSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.DeviceSelectNames";

        return await command.ReadNameds(connection);
    }
}