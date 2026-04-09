namespace Crudspa.Education.Student.Server.Sproxies;

public static class UnitSelect
{
    public static async Task<Unit?> Execute(String connection, Guid? unitId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.UnitSelect";

        command.AddParameter("@Id", unitId);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var unit = ReadUnit(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unit.LessonSummaries.Add(ReadLessonSummary(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unit.UnitBookSummaries.Add(ReadUnitBookSummary(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                unit.ChildUnits.Add(UnitSelectBySession.ReadUnit(reader));

            var objectives = new List<ObjectiveLite>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                objectives.Add(ReadObjectiveLite(reader));

            var games = new List<Game>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                games.Add(ReadGame(reader));

            var modules = new List<Module>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                modules.Add(ReadModule(reader));

            foreach (var lessonSummary in unit.LessonSummaries)
                lessonSummary.Objectives = objectives.ToObservable(x => x.LessonId.Equals(lessonSummary.Id));

            foreach (var unitBookSummary in unit.UnitBookSummaries)
            {
                unitBookSummary.Book!.Games = games.ToObservable(x => x.BookId.Equals(unitBookSummary.BookId));
                unitBookSummary.Book!.Modules = modules.ToObservable(x => x.BookId.Equals(unitBookSummary.BookId));
            }

            return unit;
        });
    }

    private static Unit ReadUnit(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            StatusId = reader.ReadGuid(2),
            GradeId = reader.ReadGuid(3),
            Image = new()
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

    private static LessonSummary ReadLessonSummary(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            ImageId = reader.ReadGuid(2),
            GuideImageId = reader.ReadGuid(3),
            GuideText = reader.ReadString(4),
            GuideAudioId = reader.ReadGuid(5),
            RequireSequentialCompletion = reader.ReadBoolean(6),
            Ordinal = reader.ReadInt32(7),
            StatusName = reader.ReadString(8),
            Image = new()
            {
                Id = reader.ReadGuid(9),
                BlobId = reader.ReadGuid(10),
                Name = reader.ReadString(11),
                Format = reader.ReadString(12),
                Width = reader.ReadInt32(13),
                Height = reader.ReadInt32(14),
                Caption = reader.ReadString(15),
            },
            ObjectiveCount = reader.ReadInt32(16),
        };
    }

    private static UnitBookSummary ReadUnitBookSummary(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            UnitId = reader.ReadGuid(1),
            BookId = reader.ReadGuid(2),
            Treatment = reader.ReadBoolean(3),
            Control = reader.ReadBoolean(4),
            Ordinal = reader.ReadInt32(5),
            Book = new()
            {
                Title = reader.ReadString(6),
                Author = reader.ReadString(7),
                CoverImageId = reader.ReadGuid(8),
                CoverImage = new()
                {
                    Id = reader.ReadGuid(9),
                    BlobId = reader.ReadGuid(10),
                    Name = reader.ReadString(11),
                    Format = reader.ReadString(12),
                    Width = reader.ReadInt32(13),
                    Height = reader.ReadInt32(14),
                    Caption = reader.ReadString(15),
                },
                HasPreface = reader.ReadBoolean(16),
                HasContent = reader.ReadBoolean(17),
                HasMap = reader.ReadBoolean(18),
            },
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

    private static Game ReadGame(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            Key = reader.ReadString(2),
            Title = reader.ReadString(3),
            IconName = reader.ReadString(4),
        };
    }

    private static Module ReadModule(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            IconName = reader.ReadString(2),
            BookId = reader.ReadGuid(3),
            StatusId = reader.ReadGuid(4),
            BinderId = reader.ReadGuid(5),
            Ordinal = reader.ReadInt32(6),
        };
    }
}