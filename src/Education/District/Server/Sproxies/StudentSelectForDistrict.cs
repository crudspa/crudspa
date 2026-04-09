namespace Crudspa.Education.District.Server.Sproxies;

public static class StudentSelectForDistrict
{
    public static async Task<IList<Student>> Execute(String connection, Guid? sessionId, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.StudentSelectForDistrict";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", assessmentId);

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