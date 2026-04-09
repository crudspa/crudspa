namespace Crudspa.Education.School.Server.Sproxies;

public static class ListenPartCompletedSelectForAssessmentAssignment
{
    public static async Task<IList<ListenPartCompleted>> Execute(String connection, Guid? assessmentAssignmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ListenPartCompletedSelectForAssessmentAssignment";

        command.AddParameter("@AssessmentAssignmentId", assessmentAssignmentId);

        return await command.ReadAll(connection, ReadListenPartCompleted);
    }

    private static ListenPartCompleted ReadListenPartCompleted(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ListenPartId = reader.ReadGuid(2),
            DeviceTimestamp = reader.ReadDateTimeOffset(3),
        };
    }
}