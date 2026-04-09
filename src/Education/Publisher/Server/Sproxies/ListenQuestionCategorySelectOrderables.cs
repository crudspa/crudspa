namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenQuestionCategorySelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenQuestionCategorySelectOrderables";

        return await command.ReadOrderables(connection);
    }
}