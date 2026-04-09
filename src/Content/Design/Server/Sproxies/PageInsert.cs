namespace Crudspa.Content.Design.Server.Sproxies;

public static class PageInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PageInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BinderId", page.BinderId);
        command.AddParameter("@TypeId", page.TypeId);
        command.AddParameter("@Title", 150, page.Title);
        command.AddParameter("@BoxId", page.Box.Id);
        command.AddParameter("@StatusId", page.StatusId);
        command.AddParameter("@GuideText", page.GuideText);
        command.AddParameter("@GuideAudioId", page.GuideAudioFile.Id);
        command.AddParameter("@ShowNotebook", page.ShowNotebook ?? false);
        command.AddParameter("@ShowGuide", page.ShowGuide ?? false);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}