namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassroomGradeSelectNames
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassroomGradeSelectNames";

        return await command.ReadOrderables(connection);
    }
}