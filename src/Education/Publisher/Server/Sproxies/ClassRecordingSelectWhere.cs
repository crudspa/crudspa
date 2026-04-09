namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ClassRecordingSelectWhere
{
    public static async Task<IList<ClassRecording>> Execute(String connection, Guid? sessionId, ClassRecordingSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ClassRecordingSelectWhere";

        search.UploadedRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@UploadedStart", search.UploadedRange.StartDateTimeOffset);
        command.AddParameter("@UploadedEnd", search.UploadedRange.EndDateTimeOffset);

        return await command.ReadAll(connection, ReadClassRecording);
    }

    private static ClassRecording ReadClassRecording(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Uploaded = reader.ReadDateTimeOffset(3),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(8),
                OptimizedBlobId = reader.ReadGuid(9),
                OptimizedFormat = reader.ReadString(10),
            },
            ImageFile = new()
            {
                Id = reader.ReadGuid(11),
                BlobId = reader.ReadGuid(12),
                Name = reader.ReadString(13),
                Format = reader.ReadString(14),
                Width = reader.ReadInt32(15),
                Height = reader.ReadInt32(16),
                Caption = reader.ReadString(17),
            },
            CategoryId = reader.ReadGuid(18),
            CategoryName = reader.ReadString(19),
            Unit = reader.ReadInt32(20),
            Lesson = reader.ReadInt32(21),
            TeacherNotes = reader.ReadString(22),
            SchoolContact = new()
            {
                Id = reader.ReadGuid(23),
                Contact = new()
                {
                    FirstName = reader.ReadString(24),
                    LastName = reader.ReadString(25),
                },
            },
            OrganizationName = reader.ReadString(26),
        };
    }
}