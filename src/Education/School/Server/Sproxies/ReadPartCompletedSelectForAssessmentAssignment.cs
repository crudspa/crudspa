namespace Crudspa.Education.School.Server.Sproxies;

public static class ReadPartCompletedSelectForAssessmentAssignment
{
    public static async Task<IList<ReadPartCompleted>> Execute(String connection, Guid? assessmentAssignmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ReadPartCompletedSelectForAssessmentAssignment";

        command.AddParameter("@AssessmentAssignmentId", assessmentAssignmentId);

        return await command.ReadAll(connection, ReadReadPartCompleted);
    }

    private static ReadPartCompleted ReadReadPartCompleted(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ReadPartId = reader.ReadGuid(2),
            DeviceTimestamp = reader.ReadDateTimeOffset(3),
        };
    }
}