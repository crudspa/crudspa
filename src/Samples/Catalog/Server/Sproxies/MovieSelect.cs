namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieSelect
{
    public static async Task<Movie?> Execute(String connection, Guid? sessionId, Movie movie)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", movie.Id);

        return await command.ReadSingle(connection, ReadMovie);
    }

    private static Movie ReadMovie(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            GenreId = reader.ReadGuid(2),
            GenreName = reader.ReadString(3),
            RatingId = reader.ReadGuid(4),
            RatingName = reader.ReadString(5),
            Released = reader.ReadDateTimeOffset(6),
            RuntimeMin = reader.ReadInt32(7),
            Score = reader.ReadSingle(8),
            Summary = reader.ReadString(9),
            PosterImageFile = new()
            {
                Id = reader.ReadGuid(10),
                BlobId = reader.ReadGuid(11),
                Name = reader.ReadString(12),
                Format = reader.ReadString(13),
                Width = reader.ReadInt32(14),
                Height = reader.ReadInt32(15),
                Caption = reader.ReadString(16),
            },
            TrailerVideoFile = new()
            {
                Id = reader.ReadGuid(17),
                BlobId = reader.ReadGuid(18),
                Name = reader.ReadString(19),
                Format = reader.ReadString(20),
                Width = reader.ReadInt32(21),
                Height = reader.ReadInt32(22),
                OptimizedStatus = reader.ReadEnum<VideoFile.OptimizationStatus>(23),
                OptimizedBlobId = reader.ReadGuid(24),
                OptimizedFormat = reader.ReadString(25),
            },
            MovieCreditCount = reader.ReadInt32(26),
        };
    }
}