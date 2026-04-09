namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentSelect
{
    public static async Task<Assessment?> Execute(String connection, Guid? sessionId, Assessment assessment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessment.Id);

        return await command.ReadSingle(connection, ReadAssessment);
    }

    private static Assessment ReadAssessment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            StatusId = reader.ReadGuid(2),
            StatusName = reader.ReadString(3),
            GradeId = reader.ReadGuid(4),
            GradeName = reader.ReadString(5),
            AvailableStart = reader.ReadDateOnly(6),
            AvailableEnd = reader.ReadDateOnly(7),
            CategoryId = reader.ReadGuid(8),
            CategoryName = reader.ReadString(9),
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(10),
                BlobId = reader.ReadGuid(11),
                Name = reader.ReadString(12),
                Format = reader.ReadString(13),
                Width = reader.ReadInt32(14),
                Height = reader.ReadInt32(15),
                Caption = reader.ReadString(16),
            },
            VocabPartCount = reader.ReadInt32(17),
            ListenPartCount = reader.ReadInt32(18),
            ReadPartCount = reader.ReadInt32(19),
        };
    }
}