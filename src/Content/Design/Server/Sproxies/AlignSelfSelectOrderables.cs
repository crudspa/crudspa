namespace Crudspa.Content.Design.Server.Sproxies;

public static class AlignSelfSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AlignSelfSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}