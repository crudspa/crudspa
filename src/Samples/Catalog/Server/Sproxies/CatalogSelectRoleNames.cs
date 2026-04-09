namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class CatalogSelectRoleNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.CatalogSelectRoleNames";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}