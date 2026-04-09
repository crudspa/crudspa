namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class BookSelect
{
    public static async Task<Book?> Execute(String connection, Guid? sessionId, Book book)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.BookSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", book.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            book = ReadBook(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                book.BookEditions.Add(ReadBookEdition(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                book.Tags.Add(reader.ReadSelectable());

            return book;
        });
    }

    private static Book ReadBook(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            Isbn = reader.ReadString(2),
            Author = reader.ReadString(3),
            GenreId = reader.ReadGuid(4),
            GenreName = reader.ReadString(5),
            Pages = reader.ReadInt32(6),
            Price = reader.ReadSingle(7),
            Summary = reader.ReadString(8),
            CoverImageFile = new()
            {
                Id = reader.ReadGuid(9),
                BlobId = reader.ReadGuid(10),
                Name = reader.ReadString(11),
                Format = reader.ReadString(12),
                Width = reader.ReadInt32(13),
                Height = reader.ReadInt32(14),
                Caption = reader.ReadString(15),
            },
            SamplePdfFile = new()
            {
                Id = reader.ReadGuid(16),
                BlobId = reader.ReadGuid(17),
                Name = reader.ReadString(18),
                Format = reader.ReadString(19),
                Description = reader.ReadString(20),
            },
            PreviewAudioFileFile = new()
            {
                Id = reader.ReadGuid(21),
                BlobId = reader.ReadGuid(22),
                Name = reader.ReadString(23),
                Format = reader.ReadString(24),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(25),
                OptimizedBlobId = reader.ReadGuid(26),
                OptimizedFormat = reader.ReadString(27),
            },
        };
    }

    private static BookEdition ReadBookEdition(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            BookTitle = reader.ReadString(2),
            FormatId = reader.ReadGuid(3),
            FormatName = reader.ReadString(4),
            Sku = reader.ReadString(5),
            Price = reader.ReadSingle(6),
            ReleasedOn = reader.ReadDateOnly(7),
            InPrint = reader.ReadBoolean(8),
            Ordinal = reader.ReadInt32(9),
        };
    }
}