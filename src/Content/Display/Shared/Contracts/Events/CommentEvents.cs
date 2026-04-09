namespace Crudspa.Content.Display.Shared.Contracts.Events;

public class CommentPayload
{
    public Guid? Id { get; set; }
    public Guid? ThreadId { get; set; }
}

public class CommentAdded : CommentPayload;

public class CommentSaved : CommentPayload;

public class CommentRemoved : CommentPayload;