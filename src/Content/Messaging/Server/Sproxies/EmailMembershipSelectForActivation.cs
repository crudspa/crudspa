namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class EmailMembershipSelectForActivation
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId, Guid? activationId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.EmailMembershipSelectForActivation";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ActivationId", activationId);
        return await command.ReadNameds(connection);
    }
}