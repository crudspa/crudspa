namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ChapterSelect
{
    public static async Task<Chapter?> Execute(String connection, Guid? sessionId, Chapter chapter)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ChapterSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", chapter.Id);

        return await command.ReadSingle(connection, ReadChapter);
    }

    private static Chapter ReadChapter(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            BookKey = reader.ReadString(2),
            Title = reader.ReadString(3),
            Ordinal = reader.ReadInt32(4),
            Binder = new()
            {
                Id = reader.ReadGuid(5),
                TypeId = reader.ReadGuid(6),
                TypeName = reader.ReadString(7),
            },
            PageCount = reader.ReadInt32(8),
        };
    }
}