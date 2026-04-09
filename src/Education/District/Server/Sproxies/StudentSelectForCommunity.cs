namespace Crudspa.Education.District.Server.Sproxies;

public static class StudentSelectForCommunity
{
    public static async Task<IList<Student>> Execute(String connection, Guid? sessionId, Guid? assessmentId, Guid? communityId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.StudentSelectForCommunity";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", assessmentId);
        command.AddParameter("@CommunityId", communityId);

        return await command.ReadAll(connection, ReadStudent);
    }

    private static Student ReadStudent(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
        };
    }
}