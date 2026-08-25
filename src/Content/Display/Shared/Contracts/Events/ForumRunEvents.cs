namespace Crudspa.Content.Display.Shared.Contracts.Events;

public class ForumRunChanged
{
    public Guid? ForumId { get; set; }
    public Guid? ThreadId { get; set; }
    public Guid? CommentId { get; set; }
}