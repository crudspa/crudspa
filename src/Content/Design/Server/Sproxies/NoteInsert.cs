namespace Crudspa.Content.Design.Server.Sproxies;

public static class NoteInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, NoteElement note)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NoteInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", note.ElementId);
        command.AddParameter("@Instructions", note.Instructions);
        command.AddParameter("@ImageFileId", note.ImageFileFile.Id);
        command.AddParameter("@RequireText", note.RequireText ?? false);
        command.AddParameter("@RequireImageSelection", note.RequireImageSelection ?? false);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}