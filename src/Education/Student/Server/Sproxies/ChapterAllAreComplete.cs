namespace Crudspa.Education.Student.Server.Sproxies;

public static class ChapterAllAreComplete
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? chapterId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ChapterAllAreComplete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ChapterId", chapterId);

        return await command.ExecuteBoolean(connection, "@AllAreComplete");
    }
}