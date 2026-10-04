namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationSelectAccess
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? portalId, Guid? groupId)
    {
        await using var command = new SqlCommand { CommandText = "ContentDesign.PublicationSelectAccess" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@GroupId", groupId);
        return await command.ExecuteScalarInt(connection) == 1;
    }
}