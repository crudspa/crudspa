namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookSeasonSelectOrderables
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookSeasonSelectOrderables";

        return await command.ReadOrderables(connection);
    }
}