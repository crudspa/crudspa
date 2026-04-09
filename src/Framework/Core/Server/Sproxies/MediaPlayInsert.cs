namespace Crudspa.Framework.Core.Server.Sproxies;

public static class MediaPlayInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, MediaPlay mediaPlay)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.MediaPlayInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AudioFileId", mediaPlay.AudioFileId);
        command.AddParameter("@VideoFileId", mediaPlay.VideoFileId);
        command.AddParameter("@Started", mediaPlay.Started);
        command.AddParameter("@Canceled", mediaPlay.Canceled);
        command.AddParameter("@Completed", mediaPlay.Completed);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}