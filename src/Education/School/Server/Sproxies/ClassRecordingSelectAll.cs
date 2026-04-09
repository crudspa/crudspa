namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassRecordingSelectAll
{
    public static async Task<IList<ClassRecording>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassRecordingSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadClassRecording);
    }

    private static ClassRecording ReadClassRecording(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Uploaded = reader.ReadDateTimeOffset(1),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(6),
                OptimizedBlobId = reader.ReadGuid(7),
                OptimizedFormat = reader.ReadString(8),
            },
            ImageFile = new()
            {
                Id = reader.ReadGuid(9),
                BlobId = reader.ReadGuid(10),
                Name = reader.ReadString(11),
                Format = reader.ReadString(12),
                Width = reader.ReadInt32(13),
                Height = reader.ReadInt32(14),
                Caption = reader.ReadString(15),
            },
            CategoryId = reader.ReadGuid(16),
            CategoryName = reader.ReadString(17),
            Unit = reader.ReadInt32(18),
            Lesson = reader.ReadInt32(19),
            TeacherNotes = reader.ReadString(20),
        };
    }
}