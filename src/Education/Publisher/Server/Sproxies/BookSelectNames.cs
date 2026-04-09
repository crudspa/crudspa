namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookSelectNames";

        return await command.ReadNameds(connection);
    }
}