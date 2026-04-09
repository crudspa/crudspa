namespace Crudspa.Content.Design.Server.Sproxies;

public static class PageTypeSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PageTypeSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}