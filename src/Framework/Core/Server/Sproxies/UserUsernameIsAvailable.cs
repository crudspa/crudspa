namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UserUsernameIsAvailable
{
    public static async Task<Boolean> Execute(String connection, String? username, Guid? portalId, Guid? userId = null)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UserUsernameIsAvailable";

        command.AddParameter("@Username", 250, username);
        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@UserId", userId);

        return await command.ExecuteBoolean(connection, "@Available");
    }
}