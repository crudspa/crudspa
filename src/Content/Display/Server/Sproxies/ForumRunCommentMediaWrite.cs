namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunCommentMediaWrite
{
    public static async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId,
        CommentMedia media, IEnumerable<Guid?> licenseIds)
    {
        await using var command = Command("ContentDisplay.ForumRunCommentMediaInsert", sessionId, media, licenseIds, false);
        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }

    public static async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId,
        CommentMedia media, IEnumerable<Guid?> licenseIds)
    {
        await using var command = Command("ContentDisplay.ForumRunCommentMediaUpdate", sessionId, media, licenseIds, true);
        await command.Execute(connection, transaction);
    }

    public static async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId,
        Guid? id, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunCommentMediaDelete" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);
        command.AddParameter("@LicenseIds", licenseIds);
        await command.Execute(connection, transaction);
    }

    private static SqlCommand Command(String commandText, Guid? sessionId, CommentMedia media,
        IEnumerable<Guid?> licenseIds, Boolean includeId)
    {
        var command = new SqlCommand { CommandText = commandText };
        command.AddParameter("@SessionId", sessionId);
        if (includeId) command.AddParameter("@Id", media.Id);
        command.AddParameter("@CommentId", media.CommentId);
        command.AddParameter("@Type", (Int32)media.Type);
        command.AddParameter("@AudioId", media.Type == CommentMedia.Types.Audio ? media.AudioFile.Id : null);
        command.AddParameter("@ImageId", media.Type == CommentMedia.Types.Image ? media.ImageFile.Id : null);
        command.AddParameter("@PdfId", media.Type == CommentMedia.Types.Pdf ? media.PdfFile.Id : null);
        command.AddParameter("@VideoId", media.Type == CommentMedia.Types.Video ? media.VideoFile.Id : null);
        command.AddParameter("@Ordinal", media.Ordinal);
        command.AddParameter("@LicenseIds", licenseIds);
        return command;
    }
}