namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BodyTemplateSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BodyTemplateSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}