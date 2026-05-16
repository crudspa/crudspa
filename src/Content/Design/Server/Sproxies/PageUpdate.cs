namespace Crudspa.Content.Design.Server.Sproxies;

public static class PageUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PageUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", page.Id);
        command.AddParameter("@TypeId", page.TypeId);
        command.AddParameter("@Title", 150, page.Title);
        command.AddParameter("@BoxId", page.Box.Id);
        command.AddParameter("@StatusId", page.StatusId);
        command.AddParameter("@GuideText", (String?)null);
        command.AddParameter("@GuideAudioId", (Guid?)null);
        command.AddParameter("@ShowNotebook", false);
        command.AddParameter("@ShowGuide", false);

        await command.Execute(connection, transaction);
    }
}