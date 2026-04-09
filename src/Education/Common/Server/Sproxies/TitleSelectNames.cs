namespace Crudspa.Education.Common.Server.Sproxies;

public static class TitleSelectNames
{
    public static async Task<IList<Orderable>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.TitleSelectNames";

        return await command.ReadOrderables(connection);
    }
}