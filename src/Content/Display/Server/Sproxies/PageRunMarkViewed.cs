namespace Crudspa.Content.Display.Server.Sproxies;

public static class PageRunMarkViewed
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Page page, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.PageRunMarkViewed";

        command.AddParameter("@Id", page.Id);
        command.AddParameter("@SessionId", sessionId);

        await command.Execute(connection, transaction);
    }
}