namespace Crudspa.Content.Design.Server.Sproxies;

public static class WrapSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.WrapSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}