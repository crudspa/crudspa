namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CommunitySelectNames
{
    public static async Task<IList<Named>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CommunitySelectNames";

        return await command.ReadNameds(connection);
    }
}