namespace Crudspa.Education.District.Server.Sproxies;

public static class StudentSelectForSchool
{
    public static async Task<IList<Student>> Execute(String connection, Guid? sessionId, Guid? assessmentId, Guid? schoolId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.StudentSelectForSchool";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", assessmentId);
        command.AddParameter("@SchoolId", schoolId);

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