namespace Crudspa.Framework.Core.Server.Sproxies;

public static class AudioFileDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AudioFile audioFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.AudioFileDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", audioFile.Id);

        await command.Execute(connection, transaction);
    }
}