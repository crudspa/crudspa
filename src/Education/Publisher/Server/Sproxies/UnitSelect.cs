namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitSelect
{
    public static async Task<Unit?> Execute(String connection, Guid? sessionId, Unit unit)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unit.Id);

        return await command.ReadSingle(connection, ReadUnit);
    }

    private static Unit ReadUnit(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            StatusId = reader.ReadGuid(2),
            StatusName = reader.ReadString(3),
            GradeId = reader.ReadGuid(4),
            GradeName = reader.ReadString(5),
            ParentId = reader.ReadGuid(6),
            ParentTitle = reader.ReadString(7),
            RequiresAchievementId = reader.ReadGuid(8),
            RequiresAchievementTitle = reader.ReadString(9),
            GeneratesAchievementId = reader.ReadGuid(10),
            GeneratesAchievementTitle = reader.ReadString(11),
            ImageFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                Width = reader.ReadInt32(16),
                Height = reader.ReadInt32(17),
                Caption = reader.ReadString(18),
            },
            GuideText = reader.ReadString(19),
            GuideAudioFile = new()
            {
                Id = reader.ReadGuid(20),
                BlobId = reader.ReadGuid(21),
                Name = reader.ReadString(22),
                Format = reader.ReadString(23),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(24),
                OptimizedBlobId = reader.ReadGuid(25),
                OptimizedFormat = reader.ReadString(26),
            },
            IntroAudioFile = new()
            {
                Id = reader.ReadGuid(27),
                BlobId = reader.ReadGuid(28),
                Name = reader.ReadString(29),
                Format = reader.ReadString(30),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(31),
                OptimizedBlobId = reader.ReadGuid(32),
                OptimizedFormat = reader.ReadString(33),
            },
            SongAudioFile = new()
            {
                Id = reader.ReadGuid(34),
                BlobId = reader.ReadGuid(35),
                Name = reader.ReadString(36),
                Format = reader.ReadString(37),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(38),
                OptimizedBlobId = reader.ReadGuid(39),
                OptimizedFormat = reader.ReadString(40),
            },
            Treatment = reader.ReadBoolean(41),
            Control = reader.ReadBoolean(42),
            Ordinal = reader.ReadInt32(43),
            LessonCount = reader.ReadInt32(44),
            UnitBookCount = reader.ReadInt32(45),
        };
    }
}