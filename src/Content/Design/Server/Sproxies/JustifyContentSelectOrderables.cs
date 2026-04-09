namespace Crudspa.Content.Design.Server.Sproxies;

public static class JustifyContentSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.JustifyContentSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}