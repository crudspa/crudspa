namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitSelectChildren
{
    public static async Task<IList<Unit>> Execute(String connection, Unit unit)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitSelectChildren";

        command.AddParameter("@Id", unit.Id);

        return await command.ReadAll(connection, ReadUnit);
    }

    private static Unit ReadUnit(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            StatusId = reader.ReadGuid(2),
            GradeId = reader.ReadGuid(3),
            ParentId = reader.ReadGuid(4),
            ImageFile = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                Width = reader.ReadInt32(9),
                Height = reader.ReadInt32(10),
                Caption = reader.ReadString(11),
            },
            IntroAudioFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(16),
                OptimizedBlobId = reader.ReadGuid(17),
                OptimizedFormat = reader.ReadString(18),
            },
            SongAudioFile = new()
            {
                Id = reader.ReadGuid(19),
                BlobId = reader.ReadGuid(20),
                Name = reader.ReadString(21),
                Format = reader.ReadString(22),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(23),
                OptimizedBlobId = reader.ReadGuid(24),
                OptimizedFormat = reader.ReadString(25),
            },
            GuideText = reader.ReadString(26),
            GuideAudioFile = new()
            {
                Id = reader.ReadGuid(27),
                BlobId = reader.ReadGuid(28),
                Name = reader.ReadString(29),
                Format = reader.ReadString(30),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(31),
                OptimizedBlobId = reader.ReadGuid(32),
                OptimizedFormat = reader.ReadString(33),
            },
            RequiresAchievementId = reader.ReadGuid(34),
            GeneratesAchievementId = reader.ReadGuid(35),
            Treatment = reader.ReadBoolean(36),
            Control = reader.ReadBoolean(37),
            Ordinal = reader.ReadInt32(38),
            StatusName = reader.ReadString(39),
            GradeName = reader.ReadString(40),
            ParentTitle = reader.ReadString(41),
            RequiresAchievementTitle = reader.ReadString(42),
            GeneratesAchievementTitle = reader.ReadString(43),
            LessonCount = reader.ReadInt32(44),
            UnitBookCount = reader.ReadInt32(45),
        };
    }
}