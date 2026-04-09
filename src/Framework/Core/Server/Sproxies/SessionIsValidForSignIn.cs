namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SessionIsValidForSignIn
{
    public static async Task<Boolean> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SessionIsValidForSignIn";

        command.AddParameter("@PortalId", portalId);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteBoolean(connection, "@Valid");
    }
}