namespace Crudspa.Content.Display.Server.Sproxies;

public static class StylesSelectForPortal
{
    public static async Task<PortalStyles?> Execute(String connection, Guid portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.StylesSelectForPortal";

        command.AddParameter("@PortalId", portalId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var portalStyles = ReadPortalStyles(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                portalStyles.Styles.Add(ReadStyle(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                portalStyles.Fonts.Add(ReadFont(reader));

            return portalStyles;
        });
    }

    private static PortalStyles ReadPortalStyles(SqlDataReader reader)
    {
        return new()
        {
            PortalId = reader.ReadGuid(0),
            Revision = reader.ReadInt32(1),
            StyleCount = reader.ReadInt32(2),
        };
    }

    private static Style ReadStyle(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContentPortalId = reader.ReadGuid(1),
            RuleId = reader.ReadGuid(2),
            ConfigJson = reader.ReadString(3),
            Rule = new()
            {
                Id = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Key = reader.ReadString(6),
                TypeId = reader.ReadGuid(7),
                DefaultValue = reader.ReadString(8),
                RuleType = new()
                {
                    Id = reader.ReadGuid(9),
                    Name = reader.ReadString(10),
                    EditorView = reader.ReadString(11),
                    DisplayView = reader.ReadString(12),
                },
            },
        };
    }

    private static Font ReadFont(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            FileFile = new()
            {
                Id = reader.ReadGuid(2),
            },
        };
    }
}