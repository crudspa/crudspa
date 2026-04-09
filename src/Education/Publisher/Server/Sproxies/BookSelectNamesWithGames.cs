namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookSelectNamesWithGames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookSelectNamesWithGames";

        return await command.ReadNameds(connection);
    }
}