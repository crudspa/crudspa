namespace Crudspa.Content.Design.Server.Sproxies;

public static class BinderPaneUpdate
{
    public static async Task Execute(String connection, Guid? sessionId, BinderPane binderPane)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BinderPaneUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PaneId", binderPane.PaneId);
        command.AddParameter("@BinderId", binderPane.BinderId);

        await command.Execute(connection);
    }
}