namespace Crudspa.Education.Student.Server.Sproxies;

public static class BookLiteSelect
{
    public static async Task<BookLite?> Execute(String connection, Book book, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.BookLiteSelect";

        command.AddParameter("@Id", book.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var bookLite = ReadBookLite(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                bookLite.Games.Add(ReadGame(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                bookLite.Modules.Add(ReadModule(reader));

            return bookLite;
        });
    }

    private static BookLite ReadBookLite(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            Author = reader.ReadString(2),
            Summary = reader.ReadString(3),
            CoverImageId = reader.ReadGuid(4),
            PrefaceBinderId = reader.ReadGuid(5),
            CoverImage = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                Width = reader.ReadInt32(10),
                Height = reader.ReadInt32(11),
                Caption = reader.ReadString(12),
            },
            HasPreface = reader.ReadBoolean(13),
            HasSomething = reader.ReadBoolean(14),
            HasMap = reader.ReadBoolean(15),
        };
    }

    private static Game ReadGame(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            Key = reader.ReadString(2),
            Title = reader.ReadString(3),
            IconName = reader.ReadString(4),
        };
    }

    private static Module ReadModule(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            IconName = reader.ReadString(2),
            BookId = reader.ReadGuid(3),
            StatusId = reader.ReadGuid(4),
            BinderId = reader.ReadGuid(5),
            Ordinal = reader.ReadInt32(6),
        };
    }
}