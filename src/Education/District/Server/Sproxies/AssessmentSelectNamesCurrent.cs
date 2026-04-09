namespace Crudspa.Education.District.Server.Sproxies;

public static class AssessmentSelectNamesCurrent
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.AssessmentSelectNamesCurrent";

        return await command.ReadNameds(connection);
    }
}