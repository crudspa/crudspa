namespace Crudspa.Content.Design.Server.Sproxies;

public static class CourseSelectForTrack
{
    public static async Task<IList<Course>> Execute(String connection, Guid? sessionId, Guid? trackId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CourseSelectForTrack";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TrackId", trackId);

        return await command.ReadAll(connection, ReadCourse);
    }

    private static Course ReadCourse(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            TrackId = reader.ReadGuid(1),
            TrackTitle = reader.ReadString(2),
            Title = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            Description = reader.ReadString(6),
            RequiresAchievementId = reader.ReadGuid(7),
            RequiresAchievementTitle = reader.ReadString(8),
            GeneratesAchievementId = reader.ReadGuid(9),
            GeneratesAchievementTitle = reader.ReadString(10),
            Ordinal = reader.ReadInt32(11),
            Binder = new()
            {
                Id = reader.ReadGuid(12),
                TypeId = reader.ReadGuid(13),
                TypeName = reader.ReadString(14),
            },
            PageCount = reader.ReadInt32(15),
        };
    }
}