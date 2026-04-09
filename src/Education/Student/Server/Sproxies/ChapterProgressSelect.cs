namespace Crudspa.Education.Student.Server.Sproxies;

public static class ChapterProgressSelect
{
    public static async Task<ChapterProgress> Execute(String connection, Guid? sessionId, Guid? chapterId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ChapterProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ChapterId", chapterId);

        var progress = await command.ReadSingle(connection, ReadChapterProgress);

        return progress ?? new()
        {
            ChapterId = chapterId,
            TimesCompleted = 0,
        };
    }

    public static ChapterProgress ReadChapterProgress(SqlDataReader reader)
    {
        return new()
        {
            StudentId = reader.ReadGuid(0),
            ChapterId = reader.ReadGuid(1),
            BookId = reader.ReadGuid(2),
            TimesCompleted = reader.ReadInt32(3),
        };
    }
}