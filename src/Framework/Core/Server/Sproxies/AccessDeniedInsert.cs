namespace Crudspa.Framework.Core.Server.Sproxies;

public static class AccessDeniedInsert
{
    public static async Task Execute(String connection, AccessDenied accessDenied)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.AccessDeniedInsert";

        command.AddParameter("@SessionId", accessDenied.SessionId);
        command.AddParameter("@EventType", 50, accessDenied.EventType);
        command.AddParameter("@PermissionId", accessDenied.PermissionId);
        command.AddParameter("@Method", 250, accessDenied.Method);

        await command.Execute(connection);
    }
}