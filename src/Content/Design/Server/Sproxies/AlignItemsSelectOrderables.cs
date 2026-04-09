namespace Crudspa.Content.Design.Server.Sproxies;

public static class AlignItemsSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AlignItemsSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}