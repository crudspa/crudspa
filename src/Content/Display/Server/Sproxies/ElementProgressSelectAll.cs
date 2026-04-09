namespace Crudspa.Content.Display.Server.Sproxies;

public static class ElementProgressSelectAll
{
    public static async Task<IList<ElementProgress>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ElementProgressSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadElementProgress);
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