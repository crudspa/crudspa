namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PortalSelectAll
{
    public static async Task<IList<Portal>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PortalSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var portals = new List<Portal>();

            while (await reader.ReadAsync())
                portals.Add(ReadPortal(reader));

            var features = new List<PortalFeature>();

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                features.Add(ReadPortalFeature(reader));

            foreach (var portal in portals)
                portal.Features = features.Where(x => x.PortalId.Equals(portal.Id)).ToObservable();

            return portals;
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