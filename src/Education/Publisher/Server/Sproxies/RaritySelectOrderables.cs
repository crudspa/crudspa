namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class RaritySelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.RaritySelectOrderables";

        return await command.ReadOrderables(connection);
    }
}