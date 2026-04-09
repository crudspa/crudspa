namespace Crudspa.Education.Student.Server.Sproxies;

public static class BookProgressSelect
{
    public static async Task<BookProgress> Execute(String connection, Guid? sessionId, Guid? bookId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.BookProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", bookId);

        var progress = await command.ReadSingle(connection, ReadBookProgress);

        return progress ?? new()
        {
            BookId = bookId,
            ContentCompletedCount = 0,
            MapCompletedCount = 0,
            PrefaceCompletedCount = 0,
        };
    }

    public static BookProgress ReadBookProgress(SqlDataReader reader)
    {
        return new()
        {
            StudentId = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            PrefaceCompletedCount = reader.ReadInt32(2),
            ContentCompletedCount = reader.ReadInt32(3),
            MapCompletedCount = reader.ReadInt32(4),
        };
    }
}