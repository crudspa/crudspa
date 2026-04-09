namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class TagSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.TagSelectOrderables";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadOrderables(connection);
    }
}