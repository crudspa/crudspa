namespace Crudspa.Education.Student.Server.Sproxies;

public static class ObjectiveSelectForLesson
{
    public static async Task<IList<Objective>> Execute(String connection, Guid? lessonId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ObjectiveSelectForLesson";

        command.AddParameter("@LessonId", lessonId);
        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadObjective);
    }

    private static Objective ReadObjective(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            BinderId = reader.ReadGuid(2),
            Ordinal = reader.ReadInt32(3),
            TrophyImage = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                Width = reader.ReadInt32(8),
                Height = reader.ReadInt32(9),
                Caption = reader.ReadString(10),
            },
        };
    }
}