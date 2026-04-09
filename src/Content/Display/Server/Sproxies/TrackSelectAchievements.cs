namespace Crudspa.Content.Display.Server.Sproxies;

public static class TrackSelectAchievements
{
    public static async Task<Track?> Execute(String connection, Guid? courseId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.TrackSelectAchievements";

        command.AddParameter("@CourseId", courseId);

        return await command.ReadSingle(connection, ReadTrack);
    }

    private static Track ReadTrack(SqlDataReader reader)
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