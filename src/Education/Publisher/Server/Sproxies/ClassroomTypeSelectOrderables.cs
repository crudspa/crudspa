namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ClassroomTypeSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ClassroomTypeSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}