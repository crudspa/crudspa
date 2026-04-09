namespace Crudspa.Education.Student.Server.Sproxies;

public static class BookProgressSelectAll
{
    public static async Task<IList<BookProgress>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.BookProgressSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadBookProgress);
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