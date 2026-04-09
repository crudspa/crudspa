namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Movie movie)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Title", 160, movie.Title);
        command.AddParameter("@GenreId", movie.GenreId);
        command.AddParameter("@RatingId", movie.RatingId);
        command.AddParameter("@Released", movie.Released);
        command.AddParameter("@RuntimeMin", movie.RuntimeMin);
        command.AddParameter("@Score", movie.Score);
        command.AddParameter("@Summary", movie.Summary);
        command.AddParameter("@PosterImageId", movie.PosterImageFile.Id);
        command.AddParameter("@TrailerVideoId", movie.TrailerVideoFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}