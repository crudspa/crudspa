namespace Crudspa.Education.Student.Server.Sproxies;

public static class StudentAchievementSelectForSession
{
    public static async Task<IList<StudentAchievement>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentAchievementSelectForSession";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadStudentAchievement);
    }

    private static StudentAchievement ReadStudentAchievement(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            StudentId = reader.ReadGuid(1),
            Earned = reader.ReadDateTimeOffset(2),
            Achievement = new()
            {
                Id = reader.ReadGuid(3),
                Title = reader.ReadString(4),
                RarityId = reader.ReadGuid(5),
                TrophyImageId = reader.ReadGuid(6),
                VisibleToStudents = reader.ReadBoolean(7),
                RarityName = reader.ReadString(8),
                TrophyImage = new()
                {
                    Id = reader.ReadGuid(9),
                    BlobId = reader.ReadGuid(10),
                    Name = reader.ReadString(11),
                    Format = reader.ReadString(12),
                    Width = reader.ReadInt32(13),
                    Height = reader.ReadInt32(14),
                    Caption = reader.ReadString(15),
                },
            },
        };
    }
}