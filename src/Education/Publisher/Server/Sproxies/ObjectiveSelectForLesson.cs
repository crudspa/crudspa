namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ObjectiveSelectForLesson
{
    public static async Task<IList<Objective>> Execute(String connection, Guid? sessionId, Guid? lessonId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ObjectiveSelectForLesson";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LessonId", lessonId);

        return await command.ReadAll(connection, ReadObjective);
    }

    private static Objective ReadObjective(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            LessonId = reader.ReadGuid(1),
            LessonTitle = reader.ReadString(2),
            Title = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            TrophyImageFile = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                Width = reader.ReadInt32(10),
                Height = reader.ReadInt32(11),
                Caption = reader.ReadString(12),
            },
            RequiresAchievementId = reader.ReadGuid(13),
            RequiresAchievementTitle = reader.ReadString(14),
            GeneratesAchievementId = reader.ReadGuid(15),
            GeneratesAchievementTitle = reader.ReadString(16),
            Ordinal = reader.ReadInt32(17),
            Binder = new()
            {
                Id = reader.ReadGuid(18),
                TypeId = reader.ReadGuid(19),
                TypeName = reader.ReadString(20),
            },
            PageCount = reader.ReadInt32(21),
        };
    }
}