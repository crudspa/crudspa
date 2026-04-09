namespace Crudspa.Content.Display.Server.Sproxies;

public static class CourseSelectRun
{
    public static async Task<Course?> Execute(String connection, Course course, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.CourseSelectRun";

        command.AddParameter("@Id", course.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadCourse);
    }

    private static Course ReadCourse(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            Description = reader.ReadString(2),
            StatusId = reader.ReadGuid(3),
            TrackId = reader.ReadGuid(4),
            Ordinal = reader.ReadInt32(5),
            TrackTitle = reader.ReadString(6),
            TrackDescription = reader.ReadString(7),
            Binder = new()
            {
                Id = reader.ReadGuid(8),
                DisplayView = reader.ReadString(9),
            },
        };
    }
}