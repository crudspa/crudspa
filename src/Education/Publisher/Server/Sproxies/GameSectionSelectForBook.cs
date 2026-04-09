namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameSectionSelectForBook
{
    public static async Task<IList<Named>> Execute(String connection, Guid? bookId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameSectionSelectForBook";

        command.AddParameter("@BookId", bookId);

        return await command.ReadNameds(connection);
    }
}