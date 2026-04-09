namespace Crudspa.Content.Design.Server.Sproxies;

public static class NoteImageInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, NoteImage noteImage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NoteImageInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@NoteId", noteImage.NoteId);
        command.AddParameter("@ImageFileId", noteImage.ImageFileId);
        command.AddParameter("@Ordinal", noteImage.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}