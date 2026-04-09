namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookSelect
{
    public static async Task<Book?> Execute(String connection, Guid? sessionId, Book book)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", book.Id);

        return await command.ReadSingle(connection, ReadBook);
    }

    private static Book ReadBook(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            StatusId = reader.ReadGuid(2),
            StatusName = reader.ReadString(3),
            Key = reader.ReadString(4),
            Author = reader.ReadString(5),
            Isbn = reader.ReadString(6),
            Lexile = reader.ReadString(7),
            SeasonId = reader.ReadGuid(8),
            SeasonName = reader.ReadString(9),
            CategoryId = reader.ReadGuid(10),
            CategoryName = reader.ReadString(11),
            RequiresAchievementId = reader.ReadGuid(12),
            RequiresAchievementTitle = reader.ReadString(13),
            Summary = reader.ReadString(14),
            CoverImageFile = new()
            {
                Id = reader.ReadGuid(15),
                BlobId = reader.ReadGuid(16),
                Name = reader.ReadString(17),
                Format = reader.ReadString(18),
                Width = reader.ReadInt32(19),
                Height = reader.ReadInt32(20),
                Caption = reader.ReadString(21),
            },
            GuideImageFile = new()
            {
                Id = reader.ReadGuid(22),
                BlobId = reader.ReadGuid(23),
                Name = reader.ReadString(24),
                Format = reader.ReadString(25),
                Width = reader.ReadInt32(26),
                Height = reader.ReadInt32(27),
                Caption = reader.ReadString(28),
            },
            ChapterCount = reader.ReadInt32(29),
            GameCount = reader.ReadInt32(30),
            ModuleCount = reader.ReadInt32(31),
            TrifoldCount = reader.ReadInt32(32),
            PrefaceCount = reader.ReadInt32(33),
        };
    }
}