namespace Crudspa.Content.Display.Server.Sproxies;

public static class CourseSelectAchievements
{
    public static async Task<Course?> Execute(String connection, Guid? objectiveId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.CourseSelectAchievements";

        command.AddParameter("@CourseId", objectiveId);

        return await command.ReadSingle(connection, ReadCourse);
    }

    private static Course ReadCourse(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            Description = reader.ReadString(2),
            GeneratesAchievement = new()
            {
                Id = reader.ReadGuid(3),
                Title = reader.ReadString(4),
                Description = reader.ReadString(5),
                ImageId = reader.ReadGuid(6),
            },
            RequiresAchievement = new()
            {
                Id = reader.ReadGuid(7),
                Title = reader.ReadString(8),
                Description = reader.ReadString(9),
                ImageId = reader.ReadGuid(10),
            },
        };
    }
}