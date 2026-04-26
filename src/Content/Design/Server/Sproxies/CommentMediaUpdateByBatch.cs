namespace Crudspa.Content.Design.Server.Sproxies;

public static class CommentMediaUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CommentMedia commentMedia)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.CommentMediaUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", commentMedia.Id);
        command.AddParameter("@CommentId", commentMedia.CommentId);
        command.AddParameter("@Type", (Int32?)commentMedia.Type);
        command.AddParameter("@AudioId", commentMedia.AudioFile.Id);
        command.AddParameter("@ImageId", commentMedia.ImageFile.Id);
        command.AddParameter("@PdfId", commentMedia.PdfFile.Id);
        command.AddParameter("@VideoId", commentMedia.VideoFile.Id);
        command.AddParameter("@Ordinal", commentMedia.Ordinal);

        await command.Execute(connection, transaction);
    }
}