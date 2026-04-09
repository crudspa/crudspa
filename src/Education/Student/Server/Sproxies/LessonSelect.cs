namespace Crudspa.Education.Student.Server.Sproxies;

public static class LessonSelect
{
    public static async Task<Lesson?> Execute(String connection, Guid? lessonId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.LessonSelect";

        command.AddParameter("@Id", lessonId);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var lesson = ReadLesson(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                lesson.Objectives.Add(ReadObjectiveLite(reader));

            return lesson;
        });
    }

    private static Lesson ReadLesson(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            StatusId = reader.ReadGuid(2),
            UnitId = reader.ReadGuid(3),
            ImageId = reader.ReadGuid(4),
            GuideImageId = reader.ReadGuid(5),
            GuideText = reader.ReadString(6),
            GuideAudioId = reader.ReadGuid(7),
            RequireSequentialCompletion = reader.ReadBoolean(8),
            Ordinal = reader.ReadInt32(9),
            GuideAudio = new()
            {
                Id = reader.ReadGuid(10),
                BlobId = reader.ReadGuid(11),
                Name = reader.ReadString(12),
                Format = reader.ReadString(13),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(14),
                OptimizedBlobId = reader.ReadGuid(15),
                OptimizedFormat = reader.ReadString(16),
            },
            GuideImage = new()
            {
                Id = reader.ReadGuid(17),
                BlobId = reader.ReadGuid(18),
                Name = reader.ReadString(19),
                Format = reader.ReadString(20),
                Width = reader.ReadInt32(21),
                Height = reader.ReadInt32(22),
                Caption = reader.ReadString(23),
            },
            Image = new()
            {
                Id = reader.ReadGuid(24),
                BlobId = reader.ReadGuid(25),
                Name = reader.ReadString(26),
                Format = reader.ReadString(27),
                Width = reader.ReadInt32(28),
                Height = reader.ReadInt32(29),
                Caption = reader.ReadString(30),
            },
            UnitTitle = reader.ReadString(31),
        };
    }

    private static ObjectiveLite ReadObjectiveLite(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            LessonId = reader.ReadGuid(2),
            BinderId = reader.ReadGuid(3),
            Ordinal = reader.ReadInt32(4),
            TrophyImage = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                Width = reader.ReadInt32(9),
                Height = reader.ReadInt32(10),
                Caption = reader.ReadString(11),
            },
        };
    }
}