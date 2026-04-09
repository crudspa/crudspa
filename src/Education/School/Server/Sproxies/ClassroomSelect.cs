namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomSelect
{
    public static async Task<Classroom?> Execute(String connection, Classroom classroom)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomSelect";

        command.AddParameter("@Id", classroom.Id);

        return await command.ReadSingle(connection, ReadClassroom);
    }

    private static Classroom ReadClassroom(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            TypeId = reader.ReadGuid(1),
            GradeId = reader.ReadGuid(2),
            OrganizationName = reader.ReadString(3),
            TypeName = reader.ReadString(4),
            GradeName = reader.ReadString(5),
            ClassroomStudentCount = reader.ReadInt32(6),
            SmallClassroom = reader.ReadBoolean(7),
        };
    }
}