namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentSelectWhere
{
    public static async Task<IList<Assessment>> Execute(String connection, Guid? sessionId, AssessmentSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentSelectWhere";

        search.AvailableStartRange.ResolveDates(search.TimeZoneId!);
        search.AvailableEndRange.ResolveDates(search.TimeZoneId!);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Status", search.Status);
        command.AddParameter("@AvailableStartStart", search.AvailableStartRange.StartDateTimeOffset);
        command.AddParameter("@AvailableStartEnd", search.AvailableStartRange.EndDateTimeOffset);
        command.AddParameter("@AvailableEndStart", search.AvailableEndRange.StartDateTimeOffset);
        command.AddParameter("@AvailableEndEnd", search.AvailableEndRange.EndDateTimeOffset);
        command.AddParameter("@Grades", search.Grades);
        command.AddParameter("@Categories", search.Categories);

        return await command.ReadAll(connection, ReadAssessment);
    }

    private static Assessment ReadAssessment(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Name = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            GradeId = reader.ReadGuid(6),
            GradeName = reader.ReadString(7),
            AvailableStart = reader.ReadDateOnly(8),
            AvailableEnd = reader.ReadDateOnly(9),
            CategoryId = reader.ReadGuid(10),
            CategoryName = reader.ReadString(11),
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                Width = reader.ReadInt32(16),
                Height = reader.ReadInt32(17),
                Caption = reader.ReadString(18),
            },
            VocabPartCount = reader.ReadInt32(19),
            ListenPartCount = reader.ReadInt32(20),
            ReadPartCount = reader.ReadInt32(21),
        };
    }
}