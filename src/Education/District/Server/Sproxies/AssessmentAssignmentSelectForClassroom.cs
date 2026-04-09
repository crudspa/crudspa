namespace Crudspa.Education.District.Server.Sproxies;

public static class AssessmentAssignmentSelectForClassroom
{
    public static async Task<IList<AssessmentAssignment>> Execute(String connection, Guid? sessionId, Guid? assessmentId, Guid? classroomId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.AssessmentAssignmentSelectForClassroom";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", assessmentId);
        command.AddParameter("@ClassroomId", classroomId);

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