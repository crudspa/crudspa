namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameActivitySelectForSection
{
    public static async Task<IList<GameActivity>> Execute(String connection, Guid? sessionId, Guid? sectionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameActivitySelectForSection";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SectionId", sectionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var gameActivities = new List<GameActivity>();

            while (await reader.ReadAsync())
                gameActivities.Add(ReadGameActivity(reader));

            await reader.NextResultAsync();

            var activities = new List<Activity>();

            while (await reader.ReadAsync())
                activities.Add(ReadActivity(reader));

            await reader.NextResultAsync();

            var activityChoices = new List<ActivityChoice>();

            while (await reader.ReadAsync())
                activityChoices.Add(ReadActivityChoice(reader));

            await reader.NextResultAsync();

            var sharedGameActivities = new List<SharedGameActivity>();

            while (await reader.ReadAsync())
                sharedGameActivities.Add(ReadSharedGameActivity(reader));

            foreach (var activity in activities)
                activity.ActivityChoices = activityChoices.ToObservable(x => x.ActivityId.Equals(activity.Id));

            foreach (var gameActivity in gameActivities)
            {
                gameActivity.Activity = activities.First(x => x.Id.Equals(gameActivity.ActivityId));
                gameActivity.SharedWith = sharedGameActivities.ToObservable(x => x.ActivityId.Equals(gameActivity.Activity.Id));
            }

            return gameActivities;
        });
    }

    private static GameActivity ReadGameActivity(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SectionId = reader.ReadGuid(1),
            SectionTitle = reader.ReadString(2),
            ThemeWord = reader.ReadString(3),
            GroupId = reader.ReadGuid(4),
            GroupName = reader.ReadString(5),
            TypeId = reader.ReadGuid(6),
            TypeName = reader.ReadString(7),
            Rigorous = reader.ReadBoolean(8),
            Multisyllabic = reader.ReadBoolean(9),
            ActivityId = reader.ReadGuid(10),
            ActivityKey = reader.ReadString(11),
            Ordinal = reader.ReadInt32(12),
        };
    }

    private static Activity ReadActivity(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Key = reader.ReadString(1),
            ActivityTypeId = reader.ReadGuid(2),
            ContentAreaId = reader.ReadGuid(3),
            ContextText = reader.ReadString(4),
            ContextAudioFile = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(9),
                OptimizedBlobId = reader.ReadGuid(10),
                OptimizedFormat = reader.ReadString(11),
            },
            ContextImageFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                Width = reader.ReadInt32(16),
                Height = reader.ReadInt32(17),
                Caption = reader.ReadString(18),
            },
            ExtraText = reader.ReadString(19),
            ActivityTypeName = reader.ReadString(20),
            ActivityTypeDisplayView = reader.ReadString(21),
            ActivityTypeCategoryName = reader.ReadString(22),
            ContentAreaName = reader.ReadString(23),
        };
    }

    private static ActivityChoice ReadActivityChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ActivityId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            AudioFileId = reader.ReadGuid(3),
            ImageFileId = reader.ReadGuid(4),
            IsCorrect = reader.ReadBoolean(5),
            Ordinal = reader.ReadInt32(6),
            ColumnOrdinal = reader.ReadInt32(7).GetValueOrDefault(),
            AudioFile = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(12),
                OptimizedBlobId = reader.ReadGuid(13),
                OptimizedFormat = reader.ReadString(14),
            },
            ImageFile = new()
            {
                Id = reader.ReadGuid(15),
                BlobId = reader.ReadGuid(16),
                Name = reader.ReadString(17),
                Format = reader.ReadString(18),
                Width = reader.ReadInt32(19),
                Height = reader.ReadInt32(20),
                Caption = reader.ReadString(21),
            },
        };
    }

    private static SharedGameActivity ReadSharedGameActivity(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ActivityId = reader.ReadGuid(1),
            SectionId = reader.ReadGuid(2),
            SectionGameId = reader.ReadGuid(3),
            SectionTitle = reader.ReadString(4),
            SectionGameKey = reader.ReadString(5),
            SectionGameBookId = reader.ReadGuid(6),
            SectionGameBookTitle = reader.ReadString(7),
        };
    }
}