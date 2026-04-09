namespace Crudspa.Content.Design.Server.Sproxies;

public static class CourseSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CourseSelectNames";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ReadNameds(connection);
    }
}