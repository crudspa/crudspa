namespace Crudspa.Education.School.Server.Sproxies;

public static class AssessmentLevelSelectNames
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.AssessmentLevelSelectNames";

        return await command.ReadOrderables(connection);
    }
}