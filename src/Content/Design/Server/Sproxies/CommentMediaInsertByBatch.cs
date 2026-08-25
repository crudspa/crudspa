namespace Crudspa.Content.Design.Server.Sproxies;

public static class CommentMediaInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CommentMedia commentMedia)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CommentMediaInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CommentId", commentMedia.CommentId);
        command.AddParameter("@Type", (Int32?)commentMedia.Type);
        command.AddParameter("@AudioId", commentMedia.Type == CommentMedia.Types.Audio ? commentMedia.AudioFile.Id : null);
        command.AddParameter("@ImageId", commentMedia.Type == CommentMedia.Types.Image ? commentMedia.ImageFile.Id : null);
        command.AddParameter("@PdfId", commentMedia.Type == CommentMedia.Types.Pdf ? commentMedia.PdfFile.Id : null);
        command.AddParameter("@VideoId", commentMedia.Type == CommentMedia.Types.Video ? commentMedia.VideoFile.Id : null);
        command.AddParameter("@Ordinal", commentMedia.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}