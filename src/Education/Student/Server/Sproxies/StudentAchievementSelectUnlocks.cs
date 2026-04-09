namespace Crudspa.Education.Student.Server.Sproxies;

public static class StudentAchievementSelectUnlocks
{
    public static async Task<StudentUnlocks?> Execute(String connection, StudentAchievement studentAchievement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.StudentAchievementSelectUnlocks";

        command.AddParameter("@StudentId", studentAchievement.StudentId);
        command.AddParameter("@AchievementId", studentAchievement.Achievement.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var unlocks = new StudentUnlocks();

            while (await reader.ReadAsync())
                unlocks.Books.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    BookCoverImage = new()
                    {
                        Id = reader.ReadGuid(2),
                        BlobId = reader.ReadGuid(3),
                        Name = reader.ReadString(4),
                        Format = reader.ReadString(5),
                        Width = reader.ReadInt32(6),
                        Height = reader.ReadInt32(7),
                        Caption = reader.ReadString(8),
                    },
                    UnitId = reader.ReadGuid(9),
                });

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unlocks.Games.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    IconName = reader.ReadString(2),
                    BookTitle = reader.ReadString(3),
                    BookCoverImage = new()
                    {
                        Id = reader.ReadGuid(4),
                        BlobId = reader.ReadGuid(5),
                        Name = reader.ReadString(6),
                        Format = reader.ReadString(7),
                        Width = reader.ReadInt32(8),
                        Height = reader.ReadInt32(9),
                        Caption = reader.ReadString(10),
                    },
                    BookId = reader.ReadGuid(11),
                    UnitId = reader.ReadGuid(12),
                });

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unlocks.Lessons.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    UnitTitle = reader.ReadString(2),
                    Image = new()
                    {
                        Id = reader.ReadGuid(3),
                        BlobId = reader.ReadGuid(4),
                        Name = reader.ReadString(5),
                        Format = reader.ReadString(6),
                        Width = reader.ReadInt32(7),
                        Height = reader.ReadInt32(8),
                        Caption = reader.ReadString(9),
                    },
                    UnitId = reader.ReadGuid(10),
                });

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unlocks.Modules.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    IconName = reader.ReadString(2),
                    BookTitle = reader.ReadString(3),
                    BookCoverImage = new()
                    {
                        Id = reader.ReadGuid(4),
                        BlobId = reader.ReadGuid(5),
                        Name = reader.ReadString(6),
                        Format = reader.ReadString(7),
                        Width = reader.ReadInt32(8),
                        Height = reader.ReadInt32(9),
                        Caption = reader.ReadString(10),
                    },
                    BookId = reader.ReadGuid(11),
                    UnitId = reader.ReadGuid(12),
                });

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unlocks.Objectives.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    LessonTitle = reader.ReadString(2),
                    UnitTitle = reader.ReadString(3),
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
                    UnitImage = new()
                    {
                        Id = reader.ReadGuid(11),
                        BlobId = reader.ReadGuid(12),
                        Name = reader.ReadString(13),
                        Format = reader.ReadString(14),
                        Width = reader.ReadInt32(15),
                        Height = reader.ReadInt32(16),
                        Caption = reader.ReadString(17),
                    },
                    LessonId = reader.ReadGuid(18),
                    UnitId = reader.ReadGuid(19),
                });

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unlocks.Trifolds.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    BookTitle = reader.ReadString(2),
                    BookCoverImage = new()
                    {
                        Id = reader.ReadGuid(3),
                        BlobId = reader.ReadGuid(4),
                        Name = reader.ReadString(5),
                        Format = reader.ReadString(6),
                        Width = reader.ReadInt32(7),
                        Height = reader.ReadInt32(8),
                        Caption = reader.ReadString(9),
                    },
                    BookId = reader.ReadGuid(10),
                    UnitId = reader.ReadGuid(11),
                });

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unlocks.Units.Add(new()
                {
                    Id = reader.ReadGuid(0),
                    Title = reader.ReadString(1),
                    Image = new()
                    {
                        Id = reader.ReadGuid(2),
                        BlobId = reader.ReadGuid(3),
                        Name = reader.ReadString(4),
                        Format = reader.ReadString(5),
                        Width = reader.ReadInt32(6),
                        Height = reader.ReadInt32(7),
                        Caption = reader.ReadString(8),
                    },
                });

            return unlocks;
        });
    }
}