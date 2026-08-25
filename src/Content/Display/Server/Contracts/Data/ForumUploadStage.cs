namespace Crudspa.Content.Display.Server.Contracts.Data;

public class ForumUploadStage
{
    public Guid? BlobId { get; set; }
    public CommentMedia.Types Type { get; set; }
    public String? Name { get; set; }
    public String? Format { get; set; }
    public String? ContentType { get; set; }
    public Int64 Bytes { get; set; }
}