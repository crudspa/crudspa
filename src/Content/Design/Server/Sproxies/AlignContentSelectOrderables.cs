namespace Crudspa.Content.Design.Server.Sproxies;

public static class AlignContentSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AlignContentSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}