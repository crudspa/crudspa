namespace Crudspa.Content.Design.Server.Sproxies;

public static class AchievementSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AchievementSelectNames";

        command.AddParameter("@PortalId", portalId);

        return await command.ReadNameds(connection);
    }
}