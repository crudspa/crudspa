namespace Crudspa.Education.Student.Server.Sproxies;

public static class ChapterProgressSelectAll
{
    public static async Task<IList<ChapterProgress>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ChapterProgressSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadChapterProgress);
    }

    public static ChapterProgress ReadChapterProgress(SqlDataReader reader)
    {
        return new()
        {
            StudentId = reader.ReadGuid(0),
            ChapterId = reader.ReadGuid(1),
            TimesCompleted = reader.ReadInt32(2),
        };
    }
}