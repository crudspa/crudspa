namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookCategorySelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookCategorySelectOrderables";

        return await command.ReadOrderables(connection);
    }
}