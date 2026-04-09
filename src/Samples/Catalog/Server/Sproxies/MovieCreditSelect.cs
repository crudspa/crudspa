namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieCreditSelect
{
    public static async Task<MovieCredit?> Execute(String connection, Guid? sessionId, MovieCredit movieCredit)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieCreditSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", movieCredit.Id);

        return await command.ReadSingle(connection, ReadMovieCredit);
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