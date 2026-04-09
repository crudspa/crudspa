namespace Crudspa.Education.District.Server.Sproxies;

public static class AssessmentAssignmentSelectForDistrict
{
    public static async Task<IList<AssessmentAssignment>> Execute(String connection, Guid? sessionId, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.AssessmentAssignmentSelectForDistrict";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadAssessmentAssignment);
    }

    private static AssessmentAssignment ReadAssessmentAssignment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            StudentId = reader.ReadGuid(2),
        };
    }
}