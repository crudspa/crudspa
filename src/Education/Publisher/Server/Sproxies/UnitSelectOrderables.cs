namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}