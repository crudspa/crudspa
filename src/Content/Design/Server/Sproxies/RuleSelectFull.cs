namespace Crudspa.Content.Design.Server.Sproxies;

public static class RuleSelectFull
{
    public static async Task<IList<RuleFull>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.RuleSelectFull";

        return await command.ReadAll(connection, ReadRule);
    }

    private static RuleFull ReadRule(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            Key = reader.ReadString(2),
            TypeId = reader.ReadGuid(3),
            DefaultValue = reader.ReadString(4),
            RuleType = new()
            {
                Id = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                EditorView = reader.ReadString(7),
                DisplayView = reader.ReadString(8),
            },
        };
    }
}