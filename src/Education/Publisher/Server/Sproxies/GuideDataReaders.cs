namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GuideDataReaders
{
    public static GuideBinder ReadGuideBinder(SqlDataReader reader, Int32 start)
    {
        return new()
        {
            Id = reader.ReadGuid(start),
            BinderId = reader.ReadGuid(start + 1),
            GuideImageId = reader.ReadGuid(start + 2),
            GuideImage = new()
            {
                Id = reader.ReadGuid(start + 3),
                BlobId = reader.ReadGuid(start + 4),
                Name = reader.ReadString(start + 5),
                Format = reader.ReadString(start + 6),
                Width = reader.ReadInt32(start + 7),
                Height = reader.ReadInt32(start + 8),
                Caption = reader.ReadString(start + 9),
            },
        };
    }

    public static GuidePage ReadGuidePage(SqlDataReader reader)
    {
        var audio = new AudioFile
        {
            Id = reader.ReadGuid(6),
            BlobId = reader.ReadGuid(7),
            Name = reader.ReadString(8),
            Format = reader.ReadString(9),
            OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(10),
            OptimizedBlobId = reader.ReadGuid(11),
            OptimizedFormat = reader.ReadString(12),
        };

        return new()
        {
            Id = reader.ReadGuid(0),
            GuideBinderId = reader.ReadGuid(1),
            PageId = reader.ReadGuid(2),
            ShowGuide = reader.ReadBoolean(3),
            GuideText = reader.ReadString(4),
            GuideAudioId = reader.ReadGuid(5),
            GuideAudioFile = audio,
            GuideAudio = audio,
            ShowNotebook = reader.ReadBoolean(13),
        };
    }
}