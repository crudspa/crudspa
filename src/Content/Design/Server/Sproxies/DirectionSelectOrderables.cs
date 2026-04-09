namespace Crudspa.Content.Design.Server.Sproxies;

public static class DirectionSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.DirectionSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}