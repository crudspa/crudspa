namespace Crudspa.Content.Design.Server.Sproxies;

public static class ElementTypeSelectFull
{
    public static async Task<IList<ElementType>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ElementTypeSelectFull";

        return await command.ReadAll(connection, ReadElementType);
    }

    private static ElementType ReadElementType(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            IconId = reader.ReadGuid(2),
            EditorView = reader.ReadString(3),
            DisplayView = reader.ReadString(4),
            RepositoryClass = reader.ReadString(5),
            OnlyChild = reader.ReadBoolean(6),
            SupportsInteraction = reader.ReadBoolean(7),
            Ordinal = reader.ReadInt32(8),
            IconCssClass = reader.ReadString(9),
        };
    }
}