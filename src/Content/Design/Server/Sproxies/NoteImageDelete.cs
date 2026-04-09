namespace Crudspa.Content.Design.Server.Sproxies;

public static class NoteImageDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, NoteImage noteImage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NoteImageDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", noteImage.Id);

        await command.Execute(connection, transaction);
    }
}