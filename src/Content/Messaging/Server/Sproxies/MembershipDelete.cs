using Membership = Crudspa.Content.Messaging.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MembershipDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Membership membership)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.MembershipDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", membership.Id);

        await command.Execute(connection, transaction);
    }
}