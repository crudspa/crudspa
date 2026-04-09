namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class FormatSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.FormatSelectOrderables";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadOrderables(connection);
    }
}