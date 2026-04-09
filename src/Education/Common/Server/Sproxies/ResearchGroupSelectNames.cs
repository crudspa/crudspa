namespace Crudspa.Education.Common.Server.Sproxies;

public static class ResearchGroupSelectNames
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ResearchGroupSelectNames";

        return await command.ReadOrderables(connection);
    }
}