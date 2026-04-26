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
        command.AddParameter("@AudioId", commentMedia.AudioFile.Id);
        command.AddParameter("@ImageId", commentMedia.ImageFile.Id);
        command.AddParameter("@PdfId", commentMedia.PdfFile.Id);
        command.AddParameter("@VideoId", commentMedia.VideoFile.Id);
        command.AddParameter("@Ordinal", commentMedia.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}