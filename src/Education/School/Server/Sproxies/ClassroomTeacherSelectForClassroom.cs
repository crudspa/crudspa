namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomTeacherSelectForClassroom
{
    public static async Task<IList<ClassroomTeacher>> Execute(String connection, Guid? classroomId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomTeacherSelectForClassroom";

        command.AddParameter("@ClassroomId", classroomId);

        return await command.ReadAll(connection, ReadClassroomTeacher);
    }

    private static ClassroomTeacher ReadClassroomTeacher(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ClassroomId = reader.ReadGuid(1),
            SchoolContactId = reader.ReadGuid(2),
            Name = reader.ReadString(3),
        };
    }
}