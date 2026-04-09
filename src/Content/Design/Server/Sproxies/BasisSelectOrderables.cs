namespace Crudspa.Content.Design.Server.Sproxies;

public static class BasisSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BasisSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}