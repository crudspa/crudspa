namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieSelectWhere
{
    public static async Task<IList<Movie>> Execute(String connection, Guid? sessionId, MovieSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieSelectWhere";

        search.ReleasedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Genres", search.Genres);
        command.AddParameter("@Ratings", search.Ratings);
        command.AddParameter("@ReleasedStart", search.ReleasedRange.StartDateTimeOffset);
        command.AddParameter("@ReleasedEnd", search.ReleasedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadMovie);
    }

    private static Movie ReadMovie(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Title = reader.ReadString(3),
            PosterImageFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                Width = reader.ReadInt32(8),
                Height = reader.ReadInt32(9),
                Caption = reader.ReadString(10),
            },
            GenreId = reader.ReadGuid(11),
            GenreName = reader.ReadString(12),
            RatingId = reader.ReadGuid(13),
            RatingName = reader.ReadString(14),
            Released = reader.ReadDateTimeOffset(15),
            RuntimeMin = reader.ReadInt32(16),
            Score = reader.ReadSingle(17),
            Summary = reader.ReadString(18),
            MovieCreditCount = reader.ReadInt32(19),
        };
    }
}