namespace Crudspa.Content.Design.Server.Sproxies;

public static class AudioDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AudioElement audio)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AudioDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", audio.Id);

        await command.Execute(connection, transaction);
    }
}