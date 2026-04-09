namespace Crudspa.Education.School.Server.Sproxies;

public static class SchoolYearSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.SchoolYearSelectNames";

        return await command.ReadNameds(connection);
    }
}