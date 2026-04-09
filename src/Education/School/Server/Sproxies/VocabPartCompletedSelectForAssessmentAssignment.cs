namespace Crudspa.Education.School.Server.Sproxies;

public static class VocabPartCompletedSelectForAssessmentAssignment
{
    public static async Task<IList<VocabPartCompleted>> Execute(String connection, Guid? assessmentAssignmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.VocabPartCompletedSelectForAssessmentAssignment";

        command.AddParameter("@AssessmentAssignmentId", assessmentAssignmentId);

        return await command.ReadAll(connection, ReadVocabPartCompleted);
    }

    private static VocabPartCompleted ReadVocabPartCompleted(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            VocabPartId = reader.ReadGuid(2),
            DeviceTimestamp = reader.ReadDateTimeOffset(3),
        };
    }
}