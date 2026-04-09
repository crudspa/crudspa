namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PermissionSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PermissionSelectNames";

        command.AddParameter("@PortalId", portalId);

        return await command.ReadNameds(connection);
    }
}