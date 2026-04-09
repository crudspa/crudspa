using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class MembershipUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Membership membership)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MembershipUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", membership.Id);
        command.AddParameter("@Name", 75, membership.Name);
        command.AddParameter("@Description", membership.Description);
        command.AddParameter("@SupportsOptOut", membership.SupportsOptOut ?? false);

        await command.Execute(connection, transaction);
    }
}