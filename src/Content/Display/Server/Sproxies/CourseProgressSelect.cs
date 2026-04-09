namespace Crudspa.Content.Display.Server.Sproxies;

public static class CourseProgressSelect
{
    public static async Task<CourseProgress> Execute(String connection, Guid? sessionId, Guid? courseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.CourseProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CourseId", courseId);

        var progress = await command.ReadSingle(connection, ReadCourseProgress);

        return progress ?? new CourseProgress
        {
            CourseId = courseId,
            TimesCompleted = 0,
        };
    }

    public static CourseProgress ReadCourseProgress(SqlDataReader reader)
    {
        return new()
        {
            ContactId = reader.ReadGuid(0),
            CourseId = reader.ReadGuid(1),
            TimesCompleted = reader.ReadInt32(2),
        };
    }
}