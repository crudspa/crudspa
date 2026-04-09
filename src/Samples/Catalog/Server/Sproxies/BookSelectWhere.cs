namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class BookSelectWhere
{
    public static async Task<IList<Book>> Execute(String connection, Guid? sessionId, BookSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.BookSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Genres", search.Genres);

        return await command.ReadAll(connection, ReadBook);
    }

    private static Book ReadBook(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Title = reader.ReadString(3),
            CoverImageFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                Width = reader.ReadInt32(8),
                Height = reader.ReadInt32(9),
                Caption = reader.ReadString(10),
            },
            Isbn = reader.ReadString(11),
            Author = reader.ReadString(12),
            GenreId = reader.ReadGuid(13),
            GenreName = reader.ReadString(14),
            Pages = reader.ReadInt32(15),
            Price = reader.ReadSingle(16),
            Summary = reader.ReadString(17),
        };
    }
}