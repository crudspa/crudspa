namespace Crudspa.Content.Design.Server.Sproxies;

public static class NoteUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, NoteElement note)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NoteUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", note.Id);
        command.AddParameter("@Instructions", note.Instructions);
        command.AddParameter("@ImageFileId", note.ImageFileFile.Id);
        command.AddParameter("@RequireText", note.RequireText ?? false);
        command.AddParameter("@RequireImageSelection", note.RequireImageSelection ?? false);

        await command.Execute(connection, transaction);
    }
}