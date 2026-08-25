using Membership = Crudspa.Content.Messaging.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MembershipInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Membership membership)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.MembershipInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", membership.PortalId);
        command.AddParameter("@Name", 75, membership.Name);
        command.AddParameter("@Description", membership.Description);
        command.AddParameter("@SupportsOptOut", membership.SupportsOptOut ?? false);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}