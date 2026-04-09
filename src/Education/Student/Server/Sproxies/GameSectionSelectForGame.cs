namespace Crudspa.Education.Student.Server.Sproxies;

public static class GameSectionSelectForGame
{
    public static async Task<IList<GameSection>> Execute(String connection, Guid? gameId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.GameSectionSelectForGame";

        command.AddParameter("@GameId", gameId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var gameSections = new List<GameSection>();

            while (await reader.ReadAsync())
                gameSections.Add(ReadGameSection(reader));

            var gameActivities = new List<GameActivity>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                gameActivities.Add(ReadGameActivity(reader));

            var activityChoices = new List<ActivityChoice>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                activityChoices.Add(PageRunSelectCommon.ReadActivityChoice(reader));

            var choicesByActivityId = activityChoices.ToLookup(x => x.ActivityId);
            var activitiesBySectionId = gameActivities.ToLookup(x => x.SectionId);

            foreach (var gameActivity in gameActivities)
                gameActivity.Activity!.ActivityChoices = choicesByActivityId[gameActivity.Activity!.Id]
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

            foreach (var gameSection in gameSections)
                gameSection.GameActivities = activitiesBySectionId[gameSection.Id].ToObservable();

            return gameSections;
        });
    }

    private static GameSection ReadGameSection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            GameId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            StatusId = reader.ReadGuid(3),
            TypeId = reader.ReadGuid(4),
            Ordinal = reader.ReadInt32(5),
            StatusName = reader.ReadString(6),
            TypeName = reader.ReadString(7),
            GameActivityCount = reader.ReadInt32(8),
        };
    }

    public static GameActivity ReadGameActivity(SqlDataReader reader)
    {
        return new()
        {
            Activity = ReadActivity(reader),
            Id = reader.ReadGuid(33),
            SectionId = reader.ReadGuid(34),
            ActivityId = reader.ReadGuid(35),
            ThemeWord = reader.ReadString(36),
            Rigorous = reader.ReadBoolean(37),
            GroupId = reader.ReadGuid(38),
            Ordinal = reader.ReadInt32(39),
            SectionTitle = reader.ReadString(40),
            SectionGameId = reader.ReadGuid(41),
            SectionGameKey = reader.ReadString(42),
            SectionGameTitle = reader.ReadString(43),
            SectionGameBookId = reader.ReadGuid(44),
        };
    }

    public static Activity ReadActivity(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Key = reader.ReadString(1),
            ActivityTypeId = reader.ReadGuid(2),
            ContentAreaId = reader.ReadGuid(3),
            StatusId = reader.ReadGuid(4),
            ContextText = reader.ReadString(5),
            ContextAudioFileId = reader.ReadGuid(6),
            ContextImageFileId = reader.ReadGuid(7),
            ExtraText = reader.ReadString(8),
            ActivityTypeName = reader.ReadString(9),
            ActivityTypeKey = reader.ReadString(10),
            ActivityTypeDisplayView = reader.ReadString(11),
            ActivityTypeCategoryKey = reader.ReadString(12),
            ActivityTypeCategoryName = reader.ReadString(13),
            ActivityTypeShuffleChoices = reader.ReadBoolean(14),
            ContentAreaName = reader.ReadString(15),
            ContentAreaKey = reader.ReadString(16),
            ContentAreaAppNavText = reader.ReadString(17),
            ContextAudioFile = new()
            {
                Id = reader.ReadGuid(18),
                BlobId = reader.ReadGuid(19),
                Name = reader.ReadString(20),
                Format = reader.ReadString(21),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(22),
                OptimizedBlobId = reader.ReadGuid(23),
                OptimizedFormat = reader.ReadString(24),
            },
            ContextImageFile = new()
            {
                Id = reader.ReadGuid(25),
                BlobId = reader.ReadGuid(26),
                Name = reader.ReadString(27),
                Format = reader.ReadString(28),
                Width = reader.ReadInt32(29),
                Height = reader.ReadInt32(30),
                Caption = reader.ReadString(31),
            },
            StatusName = reader.ReadString(32),
        };
    }
}