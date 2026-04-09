namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PortalSelect
{
    public static async Task<Portal?> Execute(String connection, Guid? sessionId, Portal portal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PortalSelect";

        command.AddParameter("@Id", portal.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            portal = ReadPortal(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                portal.Features.Add(ReadPortalFeature(reader));

            return portal;
        });
    }

    private static Portal ReadPortal(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Key = reader.ReadString(1),
            Title = reader.ReadString(2),
            SegmentCount = reader.ReadInt32(3),
        };
    }

    private static PortalFeature ReadPortalFeature(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            Key = reader.ReadString(2),
            Title = reader.ReadString(3),
            IconId = reader.ReadGuid(4),
            PermissionId = reader.ReadGuid(5),
            IconCssClass = reader.ReadString(6),
        };
    }
}