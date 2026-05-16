namespace Crudspa.Content.Design.Server.Sproxies;

public static class PagePaneUpdate
{
    public static async Task Execute(String connection, Guid? sessionId, PagePane pagePane)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PagePaneUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PaneId", pagePane.PaneId);
        command.AddParameter("@PageId", pagePane.PageId);

        await command.Execute(connection);
    }
}