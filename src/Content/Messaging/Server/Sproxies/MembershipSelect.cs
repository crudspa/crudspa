using Membership = Crudspa.Content.Messaging.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class MembershipSelect
{
    public static async Task<Membership?> Execute(String connection, Guid? sessionId, Membership membership)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.MembershipSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", membership.Id);

        return await command.ReadSingle(connection, ReadMembership);
    }

    private static Membership ReadMembership(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            PopulationId = reader.ReadGuid(2),
            OrganizationId = reader.ReadGuid(3),
            Name = reader.ReadString(4),
            Description = reader.ReadString(5),
            SupportsOptOut = reader.ReadBoolean(6),
            MemberCount = reader.ReadInt32(7),
            EmailCount = reader.ReadInt32(8),
            EmailTemplateCount = reader.ReadInt32(9),
            TokenCount = reader.ReadInt32(10),
        };
    }
}