namespace Crudspa.Education.Student.Server.Sproxies;

public static class ChapterSelect
{
    public static async Task<Chapter?> Execute(String connection, Chapter chapter, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ChapterSelect";

        command.AddParameter("@Id", chapter.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadChapter);
    }

    private static Chapter ReadChapter(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            BookId = reader.ReadGuid(2),
            BinderId = reader.ReadGuid(3),
            Ordinal = reader.ReadInt32(4),
            BinderDisplayView = reader.ReadString(5),
        };
    }
}