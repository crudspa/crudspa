namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomSelectAll
{
    public static async Task<IList<Classroom>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var classrooms = new List<Classroom>();

            while (await reader.ReadAsync())
                classrooms.Add(ReadClassroom(reader));

            await reader.NextResultAsync();

            var classroomTeachers = new List<ClassroomTeacher>();

            while (await reader.ReadAsync())
                classroomTeachers.Add(ReadClassroomTeacher(reader));

            foreach (var classroom in classrooms)
                classroom.ClassroomTeachers = classroomTeachers.ToObservable(x => x.ClassroomId.Equals(classroom.Id));

            return classrooms;
        });
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
        };
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