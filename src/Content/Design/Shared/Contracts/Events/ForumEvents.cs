namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class ForumPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class ForumAdded : ForumPayload;

public class ForumSaved : ForumPayload;

public class ForumRemoved : ForumPayload;

public class ForumsReordered : ForumPayload;