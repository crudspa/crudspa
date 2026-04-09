namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieCreditSelectForMovie
{
    public static async Task<IList<MovieCredit>> Execute(String connection, Guid? sessionId, Guid? movieId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieCreditSelectForMovie";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MovieId", movieId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var movieCredits = new List<MovieCredit>();

            while (await reader.ReadAsync())
                movieCredits.Add(ReadMovieCredit(reader));

            return movieCredits;
        });
    }

    private static MovieCredit ReadMovieCredit(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            Part = reader.ReadString(2),
            Billing = reader.ReadInt32(3),
            MovieId = reader.ReadGuid(4),
            Headliner = reader.ReadBoolean(5),
            Ordinal = reader.ReadInt32(6),
        };
    }
}