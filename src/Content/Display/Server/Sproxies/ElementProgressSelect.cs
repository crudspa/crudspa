namespace Crudspa.Content.Display.Server.Sproxies;

public static class ElementProgressSelect
{
    public static async Task<ElementProgress> Execute(String connection, Guid? sessionId, Guid? elementId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ElementProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", elementId);

        var progress = await command.ReadSingle(connection, ReadElementProgress);

        return progress ?? new ElementProgress
        {
            ElementId = elementId,
            TimesCompleted = 0,
        };
    }

    public static ElementProgress ReadElementProgress(SqlDataReader reader)
    {
        return new()
        {
            ContactId = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            TimesCompleted = reader.ReadInt32(2),
        };
    }
}