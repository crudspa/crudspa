namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadQuestionTypeSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadQuestionTypeSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}