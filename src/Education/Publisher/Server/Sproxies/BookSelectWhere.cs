namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class BookSelectWhere
{
    public static async Task<IList<Book>> Execute(String connection, Guid? sessionId, BookSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.BookSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Seasons", search.Seasons);
        command.AddParameter("@Status", search.Status);
        command.AddParameter("@Categories", search.Categories);

        return await command.ReadAll(connection, ReadBook);
    }

    private static Book ReadBook(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Title = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            Key = reader.ReadString(6),
            Author = reader.ReadString(7),
            Isbn = reader.ReadString(8),
            Lexile = reader.ReadString(9),
            SeasonId = reader.ReadGuid(10),
            SeasonName = reader.ReadString(11),
            CategoryId = reader.ReadGuid(12),
            CategoryName = reader.ReadString(13),
            RequiresAchievementId = reader.ReadGuid(14),
            RequiresAchievementTitle = reader.ReadString(15),
            Summary = reader.ReadString(16),
            CoverImageFile = new()
            {
                Id = reader.ReadGuid(17),
                BlobId = reader.ReadGuid(18),
                Name = reader.ReadString(19),
                Format = reader.ReadString(20),
                Width = reader.ReadInt32(21),
                Height = reader.ReadInt32(22),
                Caption = reader.ReadString(23),
            },
            GuideImageFile = new()
            {
                Id = reader.ReadGuid(24),
                BlobId = reader.ReadGuid(25),
                Name = reader.ReadString(26),
                Format = reader.ReadString(27),
                Width = reader.ReadInt32(28),
                Height = reader.ReadInt32(29),
                Caption = reader.ReadString(30),
            },
            ChapterCount = reader.ReadInt32(31),
            GameCount = reader.ReadInt32(32),
            ModuleCount = reader.ReadInt32(33),
            TrifoldCount = reader.ReadInt32(34),
            PrefaceCount = reader.ReadInt32(35),
        };
    }
}