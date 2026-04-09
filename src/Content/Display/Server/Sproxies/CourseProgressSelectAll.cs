namespace Crudspa.Content.Display.Server.Sproxies;

public static class CourseProgressSelectAll
{
    public static async Task<IList<CourseProgress>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.CourseProgressSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadCourseProgress);
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