namespace Crudspa.Content.Display.Server.Sproxies;

public static class SeoRouteSelect
{
    public static async Task<IList<SeoRoute>> Execute(String connection, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.SeoRouteSelect";

        command.AddParameter("@PortalId", portalId);

        return await command.ReadAll(connection, ReadRoute);
    }

    private static SeoRoute ReadRoute(SqlDataReader reader)
    {
        return new()
        {
            Path = reader.ReadString(0) ?? String.Empty,
            Title = reader.ReadString(1) ?? String.Empty,
            PageId = reader.ReadGuid(2),
            PageTitle = reader.ReadString(3),
            SeoDescription = reader.ReadString(4),
            Navigable = reader.ReadBoolean(5) ?? false,
            Mapable = reader.ReadBoolean(6) ?? false,
            IsDefault = reader.ReadBoolean(7) ?? false,
        };
    }
}