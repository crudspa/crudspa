namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomStudentSelectForClassroom
{
    public static async Task<IList<ClassroomStudent>> Execute(String connection, Guid? classroomId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomStudentSelectForClassroom";

        command.AddParameter("@ClassroomId", classroomId);

        return await command.ReadAll(connection, ReadClassroomStudent);
    }

    private static ClassroomStudent ReadClassroomStudent(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ClassroomId = reader.ReadGuid(1),
            StudentId = reader.ReadGuid(2),
            Name = reader.ReadString(3),
            SecretCode = reader.ReadString(4),
            IsTestAccount = reader.ReadBoolean(5),
        };
    }
}