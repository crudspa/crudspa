namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadQuestionCategorySelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadQuestionCategorySelectOrderables";

        return await command.ReadOrderables(connection);
    }
}