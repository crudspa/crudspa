namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UsaStateSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UsaStateSelectNames";

        return await command.ReadNameds(connection);
    }
}