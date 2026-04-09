namespace Crudspa.Education.Common.Server.Sproxies;

public static class ContentAreaSelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ContentAreaSelectNames";

        return await command.ReadNameds(connection);
    }
}