namespace Crudspa.Education.District.Server.Sproxies;

public static class AssessmentAssignmentSelect
{
    public static async Task<AssessmentAssignment?> Execute(String connection, Guid? sessionId, AssessmentAssignment assessmentAssignment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.AssessmentAssignmentSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessmentAssignment.Id);

        return await command.ReadSingle(connection, ReadAssessmentAssignment);
    }

    private static AssessmentAssignment ReadAssessmentAssignment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            StudentId = reader.ReadGuid(2),
            Assigned = reader.ReadDateTimeOffset(3),
            StartAfter = reader.ReadDateTimeOffset(4),
            EndBefore = reader.ReadDateTimeOffset(5),
            Started = reader.ReadDateTimeOffset(6),
            Completed = reader.ReadDateTimeOffset(7),
            Terminated = reader.ReadDateTimeOffset(8),
            AssessmentName = reader.ReadString(9),
            AssessmentAvailableStart = reader.ReadDateOnly(10),
            AssessmentAvailableEnd = reader.ReadDateOnly(11),
            StudentFirstName = reader.ReadString(12),
            StudentLastName = reader.ReadString(13),
            ListenPartCount = reader.ReadInt32(14),
            ListenQuestionCount = reader.ReadInt32(15),
            ListenChoiceSelectionCount = reader.ReadInt32(16),
            ListenPartCompletedCount = reader.ReadInt32(17),
            ListenTextEntryCount = reader.ReadInt32(18),
            ReadPartCount = reader.ReadInt32(19),
            ReadQuestionCount = reader.ReadInt32(20),
            ReadChoiceSelectionCount = reader.ReadInt32(21),
            ReadPartCompletedCount = reader.ReadInt32(22),
            ReadTextEntryCount = reader.ReadInt32(23),
            VocabPartCount = reader.ReadInt32(24),
            VocabQuestionCount = reader.ReadInt32(25),
            VocabChoiceSelectionCount = reader.ReadInt32(26),
            VocabPartCompletedCount = reader.ReadInt32(27),
        };
    }
}