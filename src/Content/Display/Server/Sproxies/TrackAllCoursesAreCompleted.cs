namespace Crudspa.Content.Display.Server.Sproxies;

public static class TrackAllCoursesAreCompleted
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? trackId, Guid? courseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.TrackAllCoursesAreCompleted";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TrackId", trackId);
        command.AddParameter("@CourseId", courseId);

        return await command.ExecuteBoolean(connection, "@AllAreComplete");
    }
}