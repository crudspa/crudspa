namespace Crudspa.Framework.Core.Server.Sproxies;

public static class AccessCodeInsert
{
    public static async Task Execute(String connection, Guid? sessionId, Guid? userId, AccessCode accessCode)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.AccessCodeInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@UserId", userId);
        command.AddParameter("@PortalId", accessCode.PortalId);
        command.AddParameter("@Code", accessCode.Code);
        command.AddParameter("@Expires", DateTimeOffset.Now.AddMinutes(10));

        await command.Execute(connection);
    }
}