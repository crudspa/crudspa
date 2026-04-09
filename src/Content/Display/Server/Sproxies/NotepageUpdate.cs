namespace Crudspa.Content.Display.Server.Sproxies;

public static class NotepageUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Notepage notepage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.NotepageUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", notepage.Id);
        command.AddParameter("@Text", notepage.Text);
        command.AddParameter("@SelectedImageFileId", notepage.SelectedImageFileId);

        await command.Execute(connection, transaction);
    }
}