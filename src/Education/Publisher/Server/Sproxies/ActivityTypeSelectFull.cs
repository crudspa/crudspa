namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ActivityTypeSelectFull
{
    public static async Task<IList<ActivityTypeFull>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ActivityTypeSelectFull";

        return await command.ReadAll(connection, ReadActivityType);
    }

    private static ActivityTypeFull ReadActivityType(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            Key = reader.ReadString(2),
            DisplayView = reader.ReadString(3),
            CategoryId = reader.ReadGuid(4),
            StatusId = reader.ReadGuid(5),
            ContextGuidance = reader.ReadString(6),
            ChoiceGuidance = reader.ReadString(7),
            ExtraGuidance = reader.ReadString(8),
            SupportsContextText = reader.ReadBoolean(9),
            RequiresContextText = reader.ReadBoolean(10),
            SupportsContextAudio = reader.ReadBoolean(11),
            RequiresContextAudio = reader.ReadBoolean(12),
            SupportsContextImage = reader.ReadBoolean(13),
            RequiresContextImage = reader.ReadBoolean(14),
            SupportsExtraText = reader.ReadBoolean(15),
            RequiresExtraText = reader.ReadBoolean(16),
            SupportsChoices = reader.ReadBoolean(17),
            RequiresCorrectChoices = reader.ReadBoolean(18),
            SupportsAudioChoices = reader.ReadBoolean(19),
            RequiresAudioChoices = reader.ReadBoolean(20),
            RequiresDataChoices = reader.ReadBoolean(21),
            RequiresImageChoices = reader.ReadBoolean(22),
            SupportsTextChoices = reader.ReadBoolean(23),
            RequiresTextChoices = reader.ReadBoolean(24),
            RequiresLongerTextChoices = reader.ReadBoolean(25),
            RequiresColumnOrdinal = reader.ReadBoolean(26),
            RequiresTextOrImageChoices = reader.ReadBoolean(27),
            MinChoices = reader.ReadInt32(28),
            MaxChoices = reader.ReadInt32(29),
            MinCorrectChoices = reader.ReadInt32(30),
            MaxCorrectChoices = reader.ReadInt32(31),
            ShuffleChoices = reader.ReadBoolean(32),
            CategoryName = reader.ReadString(33),
            StatusName = reader.ReadString(34),
        };
    }
}