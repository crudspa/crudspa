namespace Crudspa.Content.Design.Server.Sproxies;

public static class StyleSelect
{
    public static async Task<Style?> Execute(String connection, Guid? sessionId, Style style)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.StyleSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", style.Id);

        return await command.ReadSingle(connection, ReadStyle);
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
}