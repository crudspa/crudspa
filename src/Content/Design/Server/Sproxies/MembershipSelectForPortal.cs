using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class MembershipSelectForPortal
{
    public static async Task<IList<Membership>> Execute(String connection, Guid? sessionId, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MembershipSelectForPortal";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", portalId);

        return await command.ReadAll(connection, ReadMembership);
    }

    private static Membership ReadMembership(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            Description = reader.ReadString(3),
            SupportsOptOut = reader.ReadBoolean(4),
            MemberCount = reader.ReadInt32(5),
            EmailCount = reader.ReadInt32(6),
            EmailTemplateCount = reader.ReadInt32(7),
            TokenCount = reader.ReadInt32(8),
        };
    }
}