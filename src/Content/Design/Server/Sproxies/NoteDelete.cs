namespace Crudspa.Content.Design.Server.Sproxies;

public static class NoteDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, NoteElement note)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.NoteDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", note.Id);

        await command.Execute(connection, transaction);
    }
}