namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumPortalAuthorize
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand { CommandText = "ContentDesign.ForumPortalAuthorize" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ReadSingle(connection, reader => reader.ReadBoolean(0)) == true;
    }
}