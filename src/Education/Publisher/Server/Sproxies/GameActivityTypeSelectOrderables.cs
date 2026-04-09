namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameActivityTypeSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameActivityTypeSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}