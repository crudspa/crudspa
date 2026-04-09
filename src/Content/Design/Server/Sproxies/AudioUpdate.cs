namespace Crudspa.Content.Design.Server.Sproxies;

public static class AudioUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AudioElement audio)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AudioUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", audio.Id);
        command.AddParameter("@FileId", audio.FileFile.Id);

        await command.Execute(connection, transaction);
    }
}