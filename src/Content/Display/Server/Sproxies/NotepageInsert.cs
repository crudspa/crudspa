namespace Crudspa.Content.Display.Server.Sproxies;

public static class NotepageInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Notepage notepage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.NotepageInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@NotebookId", notepage.NotebookId);
        command.AddParameter("@NoteId", notepage.NoteId);
        command.AddParameter("@Text", notepage.Text);
        command.AddParameter("@SelectedImageFileId", notepage.SelectedImageFileId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}