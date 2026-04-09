namespace Crudspa.Content.Display.Server.Sproxies;

public static class ElementLinkFollowedInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ElementLink elementLink)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ElementLinkFollowedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", elementLink.ElementId);
        command.AddParameter("@Url", 250, elementLink.Url);

        await command.Execute(connection, transaction);
    }
}