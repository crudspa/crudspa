namespace Crudspa.Framework.Core.Server.Sproxies;

public static class VideoFileDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VideoFile videoFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.VideoFileDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", videoFile.Id);

        await command.Execute(connection, transaction);
    }
}