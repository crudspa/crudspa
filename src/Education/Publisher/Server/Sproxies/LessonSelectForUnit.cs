namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LessonSelectForUnit
{
    public static async Task<IList<Lesson>> Execute(String connection, Guid? sessionId, Guid? unitId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LessonSelectForUnit";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@UnitId", unitId);

        return await command.ReadAll(connection, ReadLesson);
    }

    private static Lesson ReadLesson(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            UnitId = reader.ReadGuid(1),
            UnitTitle = reader.ReadString(2),
            Title = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            ImageFile = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                Width = reader.ReadInt32(10),
                Height = reader.ReadInt32(11),
                Caption = reader.ReadString(12),
            },
            GuideImageFile = new()
            {
                Id = reader.ReadGuid(13),
                BlobId = reader.ReadGuid(14),
                Name = reader.ReadString(15),
                Format = reader.ReadString(16),
                Width = reader.ReadInt32(17),
                Height = reader.ReadInt32(18),
                Caption = reader.ReadString(19),
            },
            GuideText = reader.ReadString(20),
            GuideAudioFile = new()
            {
                Id = reader.ReadGuid(21),
                BlobId = reader.ReadGuid(22),
                Name = reader.ReadString(23),
                Format = reader.ReadString(24),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(25),
                OptimizedBlobId = reader.ReadGuid(26),
                OptimizedFormat = reader.ReadString(27),
            },
            RequiresAchievementId = reader.ReadGuid(28),
            RequiresAchievementTitle = reader.ReadString(29),
            RequireSequentialCompletion = reader.ReadBoolean(30),
            Treatment = reader.ReadBoolean(31),
            Control = reader.ReadBoolean(32),
            GeneratesAchievementId = reader.ReadGuid(33),
            GeneratesAchievementTitle = reader.ReadString(34),
            Ordinal = reader.ReadInt32(35),
            ObjectiveCount = reader.ReadInt32(36),
        };
    }
}