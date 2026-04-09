namespace Crudspa.Content.Design.Server.Sproxies;

public static class NoteImageUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, NoteImage noteImage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NoteImageUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", noteImage.Id);
        command.AddParameter("@NoteId", noteImage.NoteId);
        command.AddParameter("@ImageFileId", noteImage.ImageFileId);
        command.AddParameter("@Ordinal", noteImage.Ordinal);

        await command.Execute(connection, transaction);
    }
}