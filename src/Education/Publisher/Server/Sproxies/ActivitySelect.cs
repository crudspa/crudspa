namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ActivitySelect
{
    public static async Task<Activity?> Execute(String connection, Activity activity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ActivitySelect";

        command.AddParameter("@Id", activity.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            activity = ReadActivity(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                activity.ActivityChoices.Add(ReadActivityChoice(reader));

            return activity;
        });
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