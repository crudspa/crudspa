namespace Crudspa.Education.Student.Server.Sproxies;

public static class AssessmentSelectForStudent
{
    public static async Task<IList<AssessmentAssignment>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.AssessmentSelectForStudent";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadAssessmentAssignment);
    }

    private static AssessmentAssignment ReadAssessmentAssignment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            StudentId = reader.ReadGuid(2),
            Assigned = reader.ReadDateTimeOffset(3),
            Started = reader.ReadDateTimeOffset(4),
            Completed = reader.ReadDateTimeOffset(5),
            Terminated = reader.ReadDateTimeOffset(6),
            AssessmentName = reader.ReadString(7),
            AssessmentAvailableStart = reader.ReadDateOnly(8),
            AssessmentAvailableEnd = reader.ReadDateOnly(9),
            ImageFile = new()
            {
                Id = reader.ReadGuid(10),
                BlobId = reader.ReadGuid(11),
                Name = reader.ReadString(12),
                Format = reader.ReadString(13),
                Width = reader.ReadInt32(14),
                Height = reader.ReadInt32(15),
                Caption = reader.ReadString(16),
            },
        };
    }
}