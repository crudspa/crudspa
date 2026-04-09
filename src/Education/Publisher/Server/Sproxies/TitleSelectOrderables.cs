namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TitleSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TitleSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}