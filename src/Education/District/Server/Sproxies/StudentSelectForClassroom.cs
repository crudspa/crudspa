namespace Crudspa.Education.District.Server.Sproxies;

public static class StudentSelectForClassroom
{
    public static async Task<IList<Student>> Execute(String connection, Guid? sessionId, Guid? classroomId, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.StudentSelectForClassroom";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ClassroomId", classroomId);
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