namespace Crudspa.Content.Design.Server.Sproxies;

public static class BundleSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BundleSelectNames";

        return await command.ReadNameds(connection);
    }
}