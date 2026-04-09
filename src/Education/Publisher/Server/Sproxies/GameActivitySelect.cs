namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameActivitySelect
{
    public static async Task<GameActivity?> Execute(String connection, Guid? sessionId, GameActivity gameActivity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameActivitySelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", gameActivity.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            gameActivity = ReadGameActivity(reader);

            await reader.NextResultAsync();

            if (!await reader.ReadAsync())
                return null;

            gameActivity.Activity = ReadActivity(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                gameActivity.Activity.ActivityChoices.Add(ReadActivityChoice(reader));

            return gameActivity;
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
}