namespace Crudspa.Education.Student.Server.Sproxies;

public static class ObjectiveSelect
{
    public static async Task<Objective?> Execute(String connection, Objective objective, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ObjectiveSelect";

        command.AddParameter("@Id", objective.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var objective = ReadObjective(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                objective.GuideBinder.Pages.Add(GuideDataReaders.ReadGuidePage(reader));

            return objective;
        });
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
            GuideBinder = GuideDataReaders.ReadGuideBinder(reader, 9),
            LessonUnitTitle = reader.ReadString(19),
            TrophyImage = new()
            {
                Id = reader.ReadGuid(20),
                BlobId = reader.ReadGuid(21),
                Name = reader.ReadString(22),
                Format = reader.ReadString(23),
                Width = reader.ReadInt32(24),
                Height = reader.ReadInt32(25),
                Caption = reader.ReadString(26),
            },
            BinderDisplayView = reader.ReadString(27),
        };
    }
}