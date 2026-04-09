namespace Crudspa.Education.District.Server.Sproxies;

public static class AssessmentAssignmentSelectWhere
{
    public static async Task<IList<AssessmentAssignment>> Execute(String connection, AssessmentAssignmentSearch search, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.AssessmentAssignmentSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Classrooms", search.Classrooms);
        command.AddParameter("@Assessments", search.Assessments);

        return await command.ReadAll(connection, ReadAssessmentAssignment, 600);
    }

    private static AssessmentAssignment ReadAssessmentAssignment(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            AssessmentId = reader.ReadGuid(3),
            StudentId = reader.ReadGuid(4),
            StartAfter = reader.ReadDateTimeOffset(5),
            EndBefore = reader.ReadDateTimeOffset(6),
            Started = reader.ReadDateTimeOffset(7),
            Completed = reader.ReadDateTimeOffset(8),
            AssessmentName = reader.ReadString(9),
            StudentFirstName = reader.ReadString(10),
            StudentLastName = reader.ReadString(11),
            ListenPartCount = reader.ReadInt32(12),
            ListenQuestionCount = reader.ReadInt32(13),
            ListenChoiceSelectionCount = reader.ReadInt32(14),
            ListenPartCompletedCount = reader.ReadInt32(15),
            ListenTextEntryCount = reader.ReadInt32(16),
            ReadPartCount = reader.ReadInt32(17),
            ReadQuestionCount = reader.ReadInt32(18),
            ReadChoiceSelectionCount = reader.ReadInt32(19),
            ReadPartCompletedCount = reader.ReadInt32(20),
            ReadTextEntryCount = reader.ReadInt32(21),
            VocabPartCount = reader.ReadInt32(22),
            VocabQuestionCount = reader.ReadInt32(23),
            VocabChoiceSelectionCount = reader.ReadInt32(24),
            VocabPartCompletedCount = reader.ReadInt32(25),
        };
    }
}