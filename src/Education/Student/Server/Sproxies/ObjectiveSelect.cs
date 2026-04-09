namespace Crudspa.Education.Student.Server.Sproxies;

public static class ObjectiveSelect
{
    public static async Task<Objective?> Execute(String connection, Objective objective, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ObjectiveSelect";

        command.AddParameter("@Id", objective.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadObjective);
    }

    private static Objective ReadObjective(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            StatusId = reader.ReadGuid(2),
            LessonId = reader.ReadGuid(3),
            TrophyImageId = reader.ReadGuid(4),
            BinderId = reader.ReadGuid(5),
            Ordinal = reader.ReadInt32(6),
            LessonTitle = reader.ReadString(7),
            LessonUnitId = reader.ReadGuid(8),
            LessonGuideImage = new()
            {
                Id = reader.ReadGuid(9),
                BlobId = reader.ReadGuid(10),
                Name = reader.ReadString(11),
                Format = reader.ReadString(12),
                Width = reader.ReadInt32(13),
                Height = reader.ReadInt32(14),
                Caption = reader.ReadString(15),
            },
            LessonUnitTitle = reader.ReadString(16),
            TrophyImage = new()
            {
                Id = reader.ReadGuid(17),
                BlobId = reader.ReadGuid(18),
                Name = reader.ReadString(19),
                Format = reader.ReadString(20),
                Width = reader.ReadInt32(21),
                Height = reader.ReadInt32(22),
                Caption = reader.ReadString(23),
            },
            BinderDisplayView = reader.ReadString(24),
        };
    }
}