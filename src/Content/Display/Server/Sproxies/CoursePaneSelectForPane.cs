namespace Crudspa.Content.Display.Server.Sproxies;

public static class CoursePaneSelectForPane
{
    public static async Task<CoursePane?> Execute(String connection, Guid? sessionId, Guid? paneId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.CoursePaneSelectForPane";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PaneId", paneId);

        return await command.ReadSingle(connection, ReadCoursePane);
    }

    private static CoursePane ReadCoursePane(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PaneId = reader.ReadGuid(1),
            IdSource = reader.ReadInt32(2),
            CourseId = reader.ReadGuid(3),
        };
    }
}